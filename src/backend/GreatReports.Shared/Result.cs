namespace GreatReports.Shared;

public class Result
{
    protected Result(bool isSuccess, Error error)
    {
        if (isSuccess && error != Error.None)
        {
            throw new InvalidOperationException("Success result cannot have an error.");
        }

        if (!isSuccess && error == Error.None)
        {
            throw new InvalidOperationException("Failure result must have an error.");
        }

        IsSuccess = isSuccess;
        Error = error;
    }

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public Error Error { get; }

    public static Result Success()
    {
        return new Result(true, Error.None);
    }

    public static Result Failure(Error error)
    {
        return new Result(false, error);
    }

    public static Result<TValue> Success<TValue>(TValue value)
    {
        return Result<TValue>.Success(value);
    }

    public static Result<TValue> Failure<TValue>(Error error)
    {
        return Result<TValue>.Failure(error);
    }
}

public class Result<TValue> : Result
{
    private readonly TValue? _value;

    protected internal Result(TValue? value, bool isSuccess, Error error)
        : base(isSuccess, error)
    {
        _value = value;
    }

    public TValue Value
    {
        get
        {
            if (IsFailure)
            {
                throw new InvalidOperationException("The value of a failure result cannot be accessed.");
            }

            return _value!;
        }
    }

    public static implicit operator Result<TValue>(TValue? value)
    {
        return value is not null
            ? Success(value)
            : Failure<TValue>(new Error("Value.Null", "O valor fornecido é nulo."));
    }

    public static Result<TValue> Success(TValue value)
    {
        return new Result<TValue>(value, true, Error.None);
    }

    public static new Result<TValue> Failure(Error error)
    {
        return new Result<TValue>(default, false, error);
    }
}
