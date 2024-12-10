using System;

namespace ForMiraiProject.Utilities
{
    public class OperationResult
    {
        public bool IsSuccess { get; private set; }
        public string? Message { get; private set; }
        public object? Data { get; private set; }
        public string? ErrorCode { get; private set; }
        public string? ErrorDetails { get; private set; } // New for error details

        public static OperationResult Success(string message = "Operation succeeded.", object? data = null)
        {
            return new OperationResult
            {
                IsSuccess = true,
                Message = message,
                Data = data
            };
        }

        public static OperationResult Failure(string message = "Operation failed.", string? errorCode = null, string? errorDetails = null, object? data = null)
        {
            return new OperationResult
            {
                IsSuccess = false,
                Message = message,
                ErrorCode = errorCode,
                ErrorDetails = errorDetails,
                Data = data
            };
        }

        public override string ToString()
        {
            return $"IsSuccess: {IsSuccess}, Message: {Message}, ErrorCode: {ErrorCode}, ErrorDetails: {ErrorDetails}, Data: {Data}";
        }
    }
}
