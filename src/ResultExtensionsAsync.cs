// ReSharper disable InconsistentNaming
namespace RailwayFx;

public static class ResultExtensionsAsync
{
    public static async Task<TResult> MatchAsync<TValue, TResult>(this Task<Result<TValue>> @this, Func<Error, Task<TResult>> whenError,
        Func<TValue, Task<TResult>> whenSuccess)
    {
        var result = await @this;
        return result.IsError
            ? await whenError(result.Error!)
            : await whenSuccess(result.Value!);
    }

    public static async Task<Result<TResult>> MapAsync<TValue, TResult>(this Result<TValue> @this, Func<TValue, Task<TResult>> fnTransform)
    {
        if (@this.IsError) return @this.Error!;
        return Result<TResult>.Ok(await fnTransform(@this.Value!));
    }

    public static async Task<Result<TResult>> MapAsync<TValue, TResult>(this Task<Result<TValue>> @this, Func<TValue, Task<TResult>> fnTransform)
    {
        var result = await @this;
        if (result.IsError) return result.Error!;
        return Result<TResult>.Ok(await fnTransform(result.Value!));
    }

    public static async Task<Result<TResult>> BindAsync<TValue, TResult>(this Result<TValue> @this, Func<TValue, Task<Result<TResult>>> fn)
    {
        if (@this.IsError) return @this.Error!;
        return await fn(@this.Value!);
    }

    public static async Task<Result<TResult>> BindAsync<TValue, TResult>(this Task<Result<TValue>> @this, Func<TValue, Task<Result<TResult>>> fn)
    {
        var result = await @this;
        if (result.IsError) return result.Error!;
        return await fn(result.Value!);
    }

    public static async Task<Result<TSource>> TapAsync<TSource>(this Result<TSource> @this, Func<TSource, Task> whenSuccess)
    {
        if (@this.IsSuccess) await whenSuccess(@this.Value!);
        return @this;
    }

    public static async Task<Result<TSource>> TapAsync<TSource>(this Result<TSource> @this, Func<Error, Task> whenError,
        Func<TSource, Task> whenSuccess)
    {
        if (@this.IsError) await whenError(@this.Error!);

        if (@this.IsSuccess) await whenSuccess(@this.Value!);

        return @this;
    }

    public static async Task<Result<TSource>> TapAsync<TSource>(this Task<Result<TSource>> @this, Func<TSource, Task> whenSuccess)
    {
        var result = await @this;
        if (result.IsSuccess) await whenSuccess(result.Value!);
        return result;
    }

    public static async Task<Result<TSource>> TapAsync<TSource>(this Task<Result<TSource>> @this, Func<Error, Task> whenError, Func<TSource, Task> whenSuccess)
    {
        var result = await @this;
        if (result.IsError) await whenError(result.Error!);
        if (result.IsSuccess) await whenSuccess(result.Value!);
        return result;
    }
}
