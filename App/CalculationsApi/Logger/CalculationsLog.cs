namespace CalculationsApi.Logger
{
    // Keep event ids in the 1000 range for easy identification
    public static partial class CalculationsLog
    {
        [LoggerMessage(
        EventId = 1000,
        EventName = "RequestStarted",
        Level = LogLevel.Information,
        Message = "Processing calculation request, type: {calculationType}, json: {json}")]
        internal static partial void RequestStarted(
        ILogger logger,
        string calculationType,
        string json);

        [LoggerMessage(
        EventId = 1001,
        EventName = "NotFound",
        Level = LogLevel.Warning,
        Message = "The calculation requested was not found, type: {calculationType}")]
        internal static partial void NotFound(
        ILogger logger,
        string calculationType);

        [LoggerMessage(
        EventId = 1002,
        EventName = "InvalidRequest",
        Level = LogLevel.Error,
        Message = "The request provided invalid json for calculation type: {calculationType}")]
        internal static partial void InvalidRequest(
        ILogger logger,
        string calculationType);

        [LoggerMessage(
        EventId = 1003,
        EventName = "RequestComplete",
        Level = LogLevel.Information,
        Message = "Completed calculation request, type: {calculationType}, result: {result}")]
        internal static partial void RequestComplete(
        ILogger logger,
        string calculationType,
        string result);

        [LoggerMessage(
        EventId = 1004,
        EventName = "FailedValidation",
        Level = LogLevel.Error,
        Message = "The request failed validation calculation type: {calculationType}, message: {message} errors: {errors}")]
        internal static partial void FailedValidation(
        ILogger logger,
        string calculationType,
        string message,
        string errors);
    }
}
