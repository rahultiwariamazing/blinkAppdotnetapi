namespace BlinkDemoApi.Application.Common;

/// <summary>
/// Standard response envelope to keep all APIs consistent.
/// Your mobile app can rely on these fields.
/// </summary>
public sealed class ApiResponse<T>
{
    public bool Success { get; set; }
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? ErrorCode { get; set; }
    public string TraceId { get; set; } = string.Empty;
    public T? Data { get; set; }

    public static ApiResponse<T> Ok(T data, string message, int statusCode, string traceId)
        => new() { Success = true, StatusCode = statusCode, Message = message, TraceId = traceId, Data = data };

    public static ApiResponse<T> Fail(string message, int statusCode, string traceId, string? errorCode = null)
        => new() { Success = false, StatusCode = statusCode, Message = message, TraceId = traceId, ErrorCode = errorCode };
}
