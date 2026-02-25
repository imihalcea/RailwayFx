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
}