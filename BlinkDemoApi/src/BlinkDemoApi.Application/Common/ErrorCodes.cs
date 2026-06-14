namespace BlinkDemoApi.Application.Common;

/// <summary>
/// Central place for error codes.
/// The UI can switch on ErrorCode if needed.
/// </summary>
public static class ErrorCodes
{
    public const string ValidationFailed = "VALIDATION_FAILED";
    public const string DuplicateMobile = "DUPLICATE_MOBILE";
    public const string DuplicateEmail = "DUPLICATE_EMAIL";
    public const string InvalidCredentials = "INVALID_CREDENTIALS";
    public const string Unauthorized = "UNAUTHORIZED";
    public const string NotFound = "NOT_FOUND";
    public const string Conflict = "CONFLICT";
    public const string ServerError = "SERVER_ERROR";
}
