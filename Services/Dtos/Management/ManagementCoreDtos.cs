namespace CNPM.Services.Dtos;

public class OperationResultDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
}

public class DropdownItemDto
{
    public string Id { get; set; } = string.Empty;
    public string? Name { get; set; }
}

public class PagedResultDto<T>
{
    public List<T> Items { get; set; } = [];
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalItems { get; set; }
    public int TotalPages { get; set; }
    public string? Message { get; set; }
}
