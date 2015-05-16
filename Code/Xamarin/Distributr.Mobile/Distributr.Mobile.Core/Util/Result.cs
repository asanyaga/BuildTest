using System;

namespace Distributr.Mobile.Core.Util
{
    public class Result<T>
    {
        public readonly Exception exception;
        public readonly string message;
        public readonly T value;

        private Result(T value, Exception exception, String message)
        {
            this.value = value;
            this.message = message;
            this.exception = exception;
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
            return String.IsNullOrEmpty(message) && exception == null;
        }
    }
}