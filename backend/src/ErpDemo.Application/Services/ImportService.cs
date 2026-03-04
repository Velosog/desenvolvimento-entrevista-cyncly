using ErpDemo.Application.DTOs;
using ErpDemo.Application.Interfaces;
using ErpDemo.Domain.Entities;
using ErpDemo.Domain.Interfaces;

namespace ErpDemo.Application.Services;

public class ImportService : IImportService
{
    private readonly ICustomerRepository _customerRepository;

    public ImportService(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<ImportResultDto> ImportCustomersFromCsvAsync(Stream csvStream)
    {
        var result = new ImportResultDto();
        var lines = await ReadCsvLinesAsync(csvStream);

        if (lines.Count < 2)
        {
            result.Errors.Add(new ImportRowError { Row = 0, Field = "File", Message = "Arquivo CSV vazio ou sem dados." });
            return result;
        }

        var header = ParseCsvLine(lines[0]);
        var columnMap = MapColumns(header);

        if (columnMap.NameIndex < 0 || columnMap.DocumentIndex < 0 || columnMap.EmailIndex < 0)
        {
            result.Errors.Add(new ImportRowError
            {
                Row = 0,
                Field = "Header",
                Message = "CSV deve conter as colunas: Name, Document, Email. Coluna Phone é opcional."
            });
            return result;
        }

        var seenDocuments = new HashSet<string>();

        for (int i = 1; i < lines.Count; i++)
        {
            var line = lines[i].Trim();
            if (string.IsNullOrWhiteSpace(line)) continue;

            result.TotalRows++;
            var fields = ParseCsvLine(line);
            var rowNumber = i + 1;

            var name = GetField(fields, columnMap.NameIndex);
            var document = GetField(fields, columnMap.DocumentIndex);
            var email = GetField(fields, columnMap.EmailIndex);
            var phone = GetField(fields, columnMap.PhoneIndex);

            var rowErrors = ValidateRow(rowNumber, name, document, email);
            if (rowErrors.Count > 0)
            {
                result.Invalid++;
                result.Errors.AddRange(rowErrors);
                continue;
            }

            var cleanDocument = new string(document.Where(char.IsDigit).ToArray());

            if (seenDocuments.Contains(cleanDocument))
            {
                result.Duplicated++;
                result.DuplicatedDocuments.Add(cleanDocument);
                continue;
            }

            if (await _customerRepository.DocumentExistsAsync(cleanDocument))
            {
                result.Duplicated++;
                result.DuplicatedDocuments.Add(cleanDocument);
                seenDocuments.Add(cleanDocument);
                continue;
            }

            seenDocuments.Add(cleanDocument);

            var customer = new Customer
            {
                Id = Guid.NewGuid(),
                Name = name.Trim(),
                Document = cleanDocument,
                Email = email.Trim(),
                Phone = phone?.Trim() ?? string.Empty,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            await _customerRepository.AddAsync(customer);
            result.Inserted++;
        }

        return result;
    }

    private static async Task<List<string>> ReadCsvLinesAsync(Stream stream)
    {
        var lines = new List<string>();
        using var reader = new StreamReader(stream);
        while (!reader.EndOfStream)
        {
            var line = await reader.ReadLineAsync();
            if (line != null) lines.Add(line);
        }
        return lines;
    }

    private static string[] ParseCsvLine(string line)
    {
        return line.Split(',', ';').Select(f => f.Trim().Trim('"')).ToArray();
    }

    private static string GetField(string[] fields, int index)
    {
        return index >= 0 && index < fields.Length ? fields[index] : string.Empty;
    }

    private static (int NameIndex, int DocumentIndex, int EmailIndex, int PhoneIndex) MapColumns(string[] header)
    {
        var normalized = header.Select(h => h.Trim().ToLowerInvariant()).ToArray();
        return (
            NameIndex: Array.FindIndex(normalized, h => h is "name" or "nome"),
            DocumentIndex: Array.FindIndex(normalized, h => h is "document" or "documento" or "cpf" or "cnpj" or "cpf/cnpj"),
            EmailIndex: Array.FindIndex(normalized, h => h is "email" or "e-mail"),
            PhoneIndex: Array.FindIndex(normalized, h => h is "phone" or "telefone" or "tel")
        );
    }

    private static List<ImportRowError> ValidateRow(int row, string name, string document, string email)
    {
        var errors = new List<ImportRowError>();

        if (string.IsNullOrWhiteSpace(name))
            errors.Add(new ImportRowError { Row = row, Field = "Name", Message = "Nome é obrigatório" });

        if (string.IsNullOrWhiteSpace(document))
        {
            errors.Add(new ImportRowError { Row = row, Field = "Document", Message = "Documento é obrigatório" });
        }
        else
        {
            var digits = new string(document.Where(char.IsDigit).ToArray());
            if (digits.Length != 11 && digits.Length != 14)
                errors.Add(new ImportRowError { Row = row, Field = "Document", Message = "Documento inválido (CPF: 11 dígitos, CNPJ: 14 dígitos)" });
        }

        if (string.IsNullOrWhiteSpace(email))
            errors.Add(new ImportRowError { Row = row, Field = "Email", Message = "Email é obrigatório" });
        else if (!email.Contains('@'))
            errors.Add(new ImportRowError { Row = row, Field = "Email", Message = "Email inválido" });

        return errors;
    }
}
