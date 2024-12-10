using System;

namespace ForMiraiProject.Utilities
{
    public class OperationResultCollect
    {
        // Indicates whether the operation was successful.
        public bool IsSuccess { get; private set; }

        // Message providing additional context about the operation result.
        public string Message { get; private set; } = "Operation succeeded.";  // Default value

        // Data returned as a result of the operation.
        public object? ResultData { get; private set; }

        // Optional error code for more detailed error handling.
        public string? ErrorCode { get; private set; }

        // Factory method for creating a successful result.
        public static OperationResultCollect Success(string message = "Operation succeeded.", object? data = null)
        {
            return new OperationResultCollect
            {
                IsSuccess = true,
                Message = message,
                ResultData = data
            };
        }

        // Factory method for creating a failed result.
        public static OperationResultCollect Failure(string message = "Operation failed.", string? errorCode = null, object? data = null)
        {
            return new OperationResultCollect
            {
                IsSuccess = false,
                Message = message,
                ErrorCode = errorCode,
                ResultData = data
            };
        }

        // Converts the operation result to a string for logging/debugging purposes.
        public override string ToString()
        {
            return $"IsSuccess: {IsSuccess}, Message: {Message}, ErrorCode: {(ErrorCode ?? "None")}, Data: {(ResultData?.ToString() ?? "No data")}";
        }
    }
}
