namespace RailwayFx.Tests;

public class ResultExtensionsAsyncTest
{
    [Test]
    public async Task should_match_success_result()
    {
        var x = await ItReturnsSuccessResultOfAsync("success")
            .MatchAsync(
                whenError: err => Task.FromResult(err.Message),
                whenSuccess: async _ => await DummySuccessAsync("success"));
            
        Assert.That(x, Is.EqualTo("success"));
    }
    
    [Test]
    public async Task should_match_error_result()
    {
        var x = await ItReturnsErrorResultAsync("err1", "error")
            .MatchAsync(
                whenError: err => Task.FromResult(err.Message), 
                whenSuccess: async _ => await DummySuccessAsync("success"));
        Assert.That(x, Is.EqualTo("error"));
    }
    
    [Test]
    public async Task should_map_success_result()
    {
        var x = await ItReturnsSuccessResultOfAsync("a")
            .MapAsync(async s => await DummySuccessAsync(s.ToUpper()));
        Assert.That(x.GetValueOrDefault(), Is.EqualTo("A"));
    }
    
    [Test]
    public async Task should_map_error_result()
    {
        var x =  await ItReturnsErrorResultAsync("err1", "error")
            .MapAsync(async s => await DummySuccessAsync(s.ToUpper()));
        Assert.That( x.GetValueOrDefault(), Is.Null);
        Assert.That(x.Error!.Key, Is.EqualTo("err1"));
    }
    
    [Test]
    public async Task should_bind_error_result()
    {
        var x = await ItReturnsErrorResultAsync("err1", "error")
            .BindAsync(async s => await SafeTryParse(s));
        Assert.That(x.GetValueOrDefault(), Is.EqualTo(null));
        Assert.That(x.Error!.Key, Is.EqualTo("err1"));
    }

    [Test]
    public async Task should_bind_success_result()
    {
        var x = await ItReturnsSuccessResultOfAsync("1")
            .BindAsync(async s => await SafeTryParse(s));
        Assert.That(x.GetValueOrDefault(), Is.EqualTo(1));
        Assert.That(x.Error, Is.Null);
    }
    
    [Test]
    public async Task should_map_async_from_sync_success_result()
    {
        var x = await Result<string>.Ok("a")
            .MapAsync(async s => await DummySuccessAsync(s.ToUpper()));
        Assert.That(x.GetValueOrDefault(), Is.EqualTo("A"));
    }

    [Test]
    public async Task should_map_async_from_sync_error_result()
    {
        var x = await Result<string>.Err(new Error("err1", "error"))
            .MapAsync(async s => await DummySuccessAsync(s.ToUpper()));
        Assert.That(x.GetValueOrDefault(), Is.Null);
        Assert.That(x.Error!.Key, Is.EqualTo("err1"));
    }

    [Test]
    public async Task should_bind_async_from_sync_success_result()
    {
        var x = await Result<string>.Ok("1")
            .BindAsync(async s => await SafeTryParse(s));
        Assert.That(x.GetValueOrDefault(), Is.EqualTo(1));
        Assert.That(x.Error, Is.Null);
    }

    [Test]
    public async Task should_bind_async_from_sync_error_result()
    {
        var x = await Result<string>.Err(new Error("err1", "error"))
            .BindAsync(async s => await SafeTryParse(s));
        Assert.That(x.GetValueOrDefault(), Is.Null);
        Assert.That(x.Error!.Key, Is.EqualTo("err1"));
    }

    [Test]
    public async Task should_chain_sync_result_into_full_async_pipeline()
    {
        var tapped = false;
        var x = await Result<string>.Ok("a")
            .MapAsync(async s => await DummySuccessAsync(s.ToUpper()))
            .TapAsync(async _ => { await Task.CompletedTask; tapped = true; })
            .BindAsync(async s => await SafeTryParseString(s));

        Assert.That(tapped, Is.True);
        Assert.That(x.GetValueOrDefault(), Is.EqualTo("A"));
    }

    [Test]
    public async Task should_tap_success_in_async_pipeline()
    {
        var tapped = false;
        var x = await ItReturnsSuccessResultOfAsync("a")
            .MapAsync(async s => await DummySuccessAsync(s.ToUpper()))
            .TapAsync(async _ => { await Task.CompletedTask; tapped = true; });

        Assert.That(tapped, Is.True);
        Assert.That(x.GetValueOrDefault(), Is.EqualTo("A"));
    }

    [Test]
    public async Task should_not_tap_success_when_error_in_async_pipeline()
    {
        var tapped = false;
        var x = await ItReturnsErrorResultAsync("err1", "error")
            .TapAsync(async _ => { await Task.CompletedTask; tapped = true; });

        Assert.That(tapped, Is.False);
        Assert.That(x.Error!.Key, Is.EqualTo("err1"));
    }

    [Test]
    public async Task should_tap_both_error_and_success_in_async_pipeline_when_success()
    {
        var errorTapped = false;
        var successTapped = false;
        var x = await ItReturnsSuccessResultOfAsync("a")
            .TapAsync(
                async _ => { await Task.CompletedTask; errorTapped = true; },
                async _ => { await Task.CompletedTask; successTapped = true; });

        Assert.That(errorTapped, Is.False);
        Assert.That(successTapped, Is.True);
        Assert.That(x.GetValueOrDefault(), Is.EqualTo("a"));
    }

    [Test]
    public async Task should_tap_both_error_and_success_in_async_pipeline_when_error()
    {
        var errorTapped = false;
        var successTapped = false;
        var x = await ItReturnsErrorResultAsync("err1", "error")
            .TapAsync(
                async _ => { await Task.CompletedTask; errorTapped = true; },
                async _ => { await Task.CompletedTask; successTapped = true; });

        Assert.That(errorTapped, Is.True);
        Assert.That(successTapped, Is.False);
        Assert.That(x.Error!.Key, Is.EqualTo("err1"));
    }

    private static Task<Result<string>> ItReturnsErrorResultAsync(string errorKey, string errorMessage)
    {
        var error = MyTestError.Create(errorKey, errorMessage);
        return Task.FromResult(Result<string>.Err(error));
    }
    
    private static Task<Result<string>> ItReturnsSuccessResultOfAsync(string value)
    {
        return Task.FromResult(Result<string>.Ok(value));
    }


    public async Task<T> DummySuccessAsync<T>(T t)
    {
        await Task.Delay(0);
        return t;
    }
    
    public async Task<Error> DummyErrorAsync<Error>(Error e)
    {
        await Task.Delay(0);
        return e;
    }
    
    private Task<Result<int?>> SafeTryParse(string value)
    {
        var parseSucceded = int.TryParse(value, out var result);
        if (parseSucceded) return Task.FromResult(Result<int?>.Ok(result));

        var error = MyTestError.Create("parseIntError", $"{value} is not an int");
        return Task.FromResult(Result<int?>.Err(error));
    }

    private Task<Result<string>> SafeTryParseString(string value)
    {
        return Task.FromResult(Result<string>.Ok(value));
    }
}