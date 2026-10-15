namespace InventoryService.Api.Models;

public sealed class ErrorResponse
{
    public string Code { get; init; } = "Internal.Error";
    public string Message { get; init; } = "An unexpected error occurred.";
    public object? Details { get; init; }
}