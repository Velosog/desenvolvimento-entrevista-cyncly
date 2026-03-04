namespace ErpDemo.Application.DTOs;

public class ImportResultDto
{
    public int TotalRows { get; set; }
    public int Inserted { get; set; }
    public int Duplicated { get; set; }
    public int Invalid { get; set; }
    public List<ImportRowError> Errors { get; set; } = new();
    public List<string> DuplicatedDocuments { get; set; } = new();
}

public class ImportRowError
{
    public int Row { get; set; }
    public string Field { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}
