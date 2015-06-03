using System;

namespace Distributr.Mobile.Core.Util
{
    public class Result<T>
    {
        public readonly Exception Exception;
        public readonly string Message;
        public readonly T Value;

        private Result(T value, Exception exception, String message)
        {
            Value = value;
            Message = message;
            Exception = exception;
        }

        public static Result<T> Success(T value)
        {
            return new Result<T>(value, null, string.Empty);
        }

        public static Result<T> Failure(Exception exception, string message)
        {
            return new Result<T>(default(T), exception, message);
        }

        public static Result<T> Failure(string message)
        {
            return new Result<T>(default(T), null, message);
        }

        public static Result<T> Failure(T value, string message)
        {
            return new Result<T>(value, null, message);
        }

        public bool WasSuccessful()
        {
            return String.IsNullOrEmpty(Message) && Exception == null;
        }
    }
}