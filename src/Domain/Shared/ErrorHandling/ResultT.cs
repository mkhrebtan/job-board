﻿namespace Domain.Shared.ErrorHandling;

public class Result<T>
{
    private Result(bool isSuccess, T value, Error error)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
    }

    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    public T Value { get; }

    public Error Error { get; }

    public static Result<T> Success(T value) => new(true, value, Error.None);

    public static Result<T> Failure(Error error) => new(false, default!, error);

    public Result ToStandardResult()
    {
        if (IsSuccess)
        {
            return Result.Success();
        }

        return Result.Failure(Error);
    }
}
