// ReSharper disable InconsistentNaming
namespace RailwayFx;

public record Error(string Key, string Message);

public class Result<TValue> : IEquatable<Result<TValue>>
{
    private Result(Error exception)
    {
        Error = exception;
        Value = default;
    }

    private Result(TValue value)
    {
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
                   (Value != null ? EqualityComparer<TValue>.Default.GetHashCode(Value):0);
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

public static class ResultExtensions
{
    public static async Task<TResult> MatchAsync<TValue, TResult>(this Task<Result<TValue>> @this, Func<Error, Task<TResult>> whenError,
        Func<TValue, Task<TResult>> whenSuccess)
    {
        return await (await @this).MatchAsync(whenError, whenSuccess);
    } 

    public static Result<TResult> Map<TValue, TResult>(this Result<TValue> @this, Func<TValue, TResult> fnTransform)
    {
        return @this.Match(Result<TResult>.Err, value => fnTransform(value));
    }

    public static async Task<Result<TResult>> MapAsync<TValue, TResult>(this Task<Result<TValue>> @this, Func<TValue, Task<TResult>> fnTransform)
    {
        return await @this.MatchAsync(
            whenError: err => Task.FromResult(Result<TResult>.Err(err)),
            whenSuccess: async value => Result<TResult>.Ok(await fnTransform(value)));
    }
    
    public static Result<TResult> Bind<TValue, TResult>(this Result<TValue> @this, Func<TValue, Result<TResult>> fn)
    {
        return @this.Match(Result<TResult>.Err, fn);
    }

    public static async Task<Result<TResult>> BindAsync<TValue, TResult>(this Task<Result<TValue>> @this, Func<TValue, Task<Result<TResult>>> fn)
    {
        return await @this.MatchAsync(
            whenError: err => Task.FromResult(Result<TResult>.Err(err)),
            whenSuccess:fn);
    }

    public static Result<TSource> Tap<TSource>(this Result<TSource> @this, Action<TSource> whenSuccess)
    {
        return @this.Tap(_ => { }, whenSuccess);
    }

    public static async Task<Result<TSource>> TapAsync<TSource>(this Result<TSource> @this, Func<TSource, Task> whenSuccess)
    {
        return await @this.TapAsync(_ =>  Task.CompletedTask , whenSuccess);
    }
    
    public static Result<TSource> Tap<TSource>(this Result<TSource> @this, Action<Error> whenError,
        Action<TSource> whenSuccess)
    {
        if (@this.IsError) whenError(@this.Error!);

        if (@this.IsSuccess) whenSuccess(@this.Value!);

        return @this;
    }

    public static async Task<Result<TSource>> TapAsync<TSource>(this Result<TSource> @this, Func<Error, Task> whenError,
        Func<TSource, Task> whenSuccess)
    {
        if (@this.IsError) await whenError(@this.Error!);

        if (@this.IsSuccess) await whenSuccess(@this.Value!);

        return @this;
    }

    public static Result<TResult> Select<TValue, TResult>(this Result<TValue> @this, Func<TValue, TResult> fnTransform)
    {
        return @this.Map(fnTransform);
    }

    public static Result<TResult> SelectMany<TValue, TResult>(this Result<TValue> @this,
        Func<TValue, Result<TResult>> fn)
    {
        return @this.Bind(fn);
    }

    public static Result<TResult2> SelectMany<TValue, TResult, TResult2>(this Result<TValue> @this,
        Func<TValue, Result<TResult>> fn, Func<TValue, TResult, TResult2> comp)
    {
        return @this.Bind(t => fn(t).Bind<TResult, TResult2>(u => comp(t, u)));
    }

    public static IEnumerable<T?> Values<T>(this IEnumerable<Result<T>> @this)
    {
        return @this.Where(r => r.IsSuccess).Select(r => r.Value);
    }

    public static IEnumerable<Error> Errors<T>(this IEnumerable<Result<T>> @this)
    {
        return @this.Where(r => r.IsError).Select(r => r.Error!);
    }


    public static (IEnumerable<Error> errors, IEnumerable<T?> outcome) SeparateResults<T>(
        this IEnumerable<Result<T>> results)
    {
        var errors = new List<Error>();
        var outcome = new List<T?>();
        foreach (var result in results)
        {
            if (result.IsError)
            {
                errors.Add(result.Error!);
            }
            else
            {
                outcome.Add(result.GetValueOrDefault());
            }
        }

        return (errors, outcome);
    }

    public static Result<T> RCast<T>(this object source)
    {
        if (source is T sourceAsT)
            return Result<T>.Ok(sourceAsT);

        return Result<T>.Err(new Error("Invalid cast",
            $"{source.GetType().Name} cannot be cast into {typeof(T).Name}"));
    }

    public static Result<T> RInvoke<T>(this Func<T> f, Func<Exception, Error> errorFunc)
    {
        try
        {
            return f();
        }
        catch (Exception e)
        {
            return errorFunc(e);
        }
    }

    public static Result<T> ThrowOnError<T>(this Result<T> @this)
    {
        if (@this.IsError) throw new InvalidOperationException($"Result is in error state: [{@this.Error!.Key}] {@this.Error!.Message}");
        return @this;
    }
}