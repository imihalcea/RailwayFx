// ReSharper disable InconsistentNaming
namespace RailwayFx;

public class Result<TValue> : IEquatable<Result<TValue>>
{
    private Result(Error error)
    {
        ArgumentNullException.ThrowIfNull(error);
        Error = error;
        Value = default;
    }

    private Result(TValue value)
    {
        ArgumentNullException.ThrowIfNull(value);
        Error = null;
        Value = value;
    }

    public bool IsSuccess => Error == null;
    public bool IsError => Error != null;

    public TValue? Value { get; }

    public Error? Error { get; }

    public static Result<TValue> Err(Error error)
    {
        return new Result<TValue>(error);
    }

    public static Result<TValue> Ok(TValue value)
    {
        return new Result<TValue>(value);
    }

    public TValue GetValueOrDefault(TValue defaultValue)
    {
        return IsSuccess ? Value! : defaultValue;
    }

    public TValue? GetValueOrDefault()
    {
        return IsSuccess ? Value : default;
    }

    public TValue Unwrap()
    {
        return IsSuccess ? Value! : throw new InvalidOperationException($"Result is in error state: [{Error!.Key}] {Error!.Message}");
    }

    public static implicit operator Result<TValue>(Error error)
    {
        return new Result<TValue>(error);
    }

    public static implicit operator Result<TValue>(TValue value)
    {
        return new Result<TValue>(value);
    }

    public TResult Match<TResult>(Func<Error, TResult> whenError, Func<TValue, TResult> whenSuccess)
    {
        return IsError ? whenError(Error!) : whenSuccess(Value!);
    }

    public Task<TResult> MatchAsync<TResult>(Func<Error, Task<TResult>> whenError,
        Func<TValue, Task<TResult>> whenSuccess)
    {
        return IsError ? whenError(Error!) : whenSuccess(Value!);
    }

    public bool Equals(Result<TValue>? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Equals(Error, other.Error) &&
               Equals(Value, other.Value);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((Result<TValue>)obj);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            return ((Error != null ? Error.GetHashCode() : 0) * 397) ^
                   (Value != null ? EqualityComparer<TValue>.Default.GetHashCode(Value) : 0);
        }
    }

    public static bool operator ==(Result<TValue> left, Result<TValue> right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(Result<TValue> left, Result<TValue> right)
    {
        return !Equals(left, right);
    }

    public override string ToString()
    {
        return IsSuccess ? $"Success[{Value}]" : $"Error[{Error!.Message}]";
    }
}
