using ErpDemo.Application.DTOs;
using FluentValidation;

namespace ErpDemo.Application.Validators;

public class CreateCustomerValidator : AbstractValidator<CreateCustomerDto>
{
    public CreateCustomerValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nome é obrigatório")
            .MaximumLength(200).WithMessage("Nome deve ter no máximo 200 caracteres");

        RuleFor(x => x.Document)
            .NotEmpty().WithMessage("Documento é obrigatório")
            .Must(BeValidDocument).WithMessage("Documento inválido (CPF: 11 dígitos, CNPJ: 14 dígitos)");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email é obrigatório")
            .EmailAddress().WithMessage("Email inválido");

        RuleFor(x => x.Phone)
            .MaximumLength(20).WithMessage("Telefone deve ter no máximo 20 caracteres");
    }

    private bool BeValidDocument(string document)
    {
        if (string.IsNullOrEmpty(document)) return false;
        var digits = new string(document.Where(char.IsDigit).ToArray());
        return digits.Length == 11 || digits.Length == 14;
    }
}
