// ReSharper disable InconsistentNaming
namespace RailwayFx;

public static class ResultExtensions
{
    public static Result<TResult> Map<TValue, TResult>(this Result<TValue> @this, Func<TValue, TResult> fnTransform)
    {
        if (@this.IsError) return @this.Error!;
        return fnTransform(@this.Value!);
    }

    public static Result<TResult> Bind<TValue, TResult>(this Result<TValue> @this, Func<TValue, Result<TResult>> fn)
    {
        if (@this.IsError) return @this.Error!;
        return fn(@this.Value!);
    }

    public static Result<TSource> Tap<TSource>(this Result<TSource> @this, Action<TSource> whenSuccess)
    {
        if (@this.IsSuccess) whenSuccess(@this.Value!);
        return @this;
    }

    public static Result<TSource> Tap<TSource>(this Result<TSource> @this, Action<Error> whenError,
        Action<TSource> whenSuccess)
    {
        if (@this.IsError) whenError(@this.Error!);

        if (@this.IsSuccess) whenSuccess(@this.Value!);

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

    public static IEnumerable<T> Values<T>(this IEnumerable<Result<T>> @this)
    {
        return @this.Where(r => r.IsSuccess).Select(r => r.Value!);
    }

    public static IEnumerable<Error> Errors<T>(this IEnumerable<Result<T>> @this)
    {
        return @this.Where(r => r.IsError).Select(r => r.Error!);
    }

    public static (IEnumerable<Error> errors, IEnumerable<T> outcome) SeparateResults<T>(
        this IEnumerable<Result<T>> results)
    {
        var errors = new List<Error>();
        var outcome = new List<T>();
        foreach (var result in results)
        {
            if (result.IsError)
            {
                errors.Add(result.Error!);
            }
            else
            {
                outcome.Add(result.Value!);
            }
        }

        return (errors, outcome);
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
