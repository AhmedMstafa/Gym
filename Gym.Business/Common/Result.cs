using System;

namespace Gym.Business.Common
{
    public class Result
    {
        protected Result(bool isSuccess, Error error)
        {
            if (isSuccess && error != Error.None || !isSuccess && error == Error.None)
            {
                throw new InvalidOperationException("Invalid error state for Result.");
            }

            IsSuccess = isSuccess;
            Error = error;
        }

        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public Error Error { get; }

        public static Result Success() => new(true, Error.None);
        public static Result Failure(Error error) => new(false, error);
        public static Result Failure(string code, string description) => new(false, new Error(code, description));

        public static Result<T> Success<T>(T value) => new(value, true, Error.None);
        public static Result<T> Failure<T>(Error error) => new(default, false, error);
        public static Result<T> Failure<T>(string code, string description) => new(default, false, new Error(code, description));
    }

    public class Result<T> : Result
    {
        private readonly T? _value;

        protected internal Result(T? value, bool isSuccess, Error error) : base(isSuccess, error)
        {
            _value = value;
        }

        public T Value => IsSuccess
            ? _value!
            : throw new InvalidOperationException("Cannot access value of a failed result.");

        public static implicit operator Result<T>(T? value) =>
            value is not null ? Success(value) : Failure<T>(new Error("NullValue", "The specified value is null."));
    }
}
