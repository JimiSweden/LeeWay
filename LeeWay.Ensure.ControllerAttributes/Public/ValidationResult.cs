using System;
using System.Reflection;

namespace LeeWay.Ensure.ControllerAttributes.Public
{

    /// <summary>
    /// Contains message, state of valid, exception from validation if failed
    /// <br/>
    /// TODO: ? probably . create Success and Fail sub classes?
    /// todo: Perhaps remove dependency on FluentAssertions?
    /// </summary>
    public sealed class ValidationResult
    {
        public bool IsValid { get; }
        public bool IsInValid => !IsValid;

        public string Message { get; }
        public Exception ExceptionFromFluentAssertions { get; set; }
        public string StackTraceFromFluentAssertions => ExceptionFromFluentAssertions?.StackTrace;
        public ValidationResult(bool isValid)
        {
            IsValid = isValid;
        }
        public ValidationResult(bool isValid, string message)
        {
            IsValid = isValid;
            Message = message;
        }

        public void SetException(Exception exception)
        {
            ExceptionFromFluentAssertions = exception;
        }
    }
}
