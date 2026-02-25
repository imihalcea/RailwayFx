namespace RailwayFx.Tests;

public class BindTests
{
    [Test]
    public void should_bind_success_result()
    {
        var x = Result<string>.Ok("1")
            .Bind(s => SafeTryParse(s));
        Assert.That(x.GetValueOrDefault(), Is.EqualTo(1));
        Assert.That(x.Error, Is.Null);
    }

    [Test]
    public void should_bind_error_result()
    {
        var x = Result<string>.Err(MyTestError.Create("err1", "error"))
            .Bind(s => SafeTryParse(s));
        Assert.That(x.GetValueOrDefault(), Is.EqualTo(0));
        Assert.That(x.Error!.Key, Is.EqualTo("err1"));
    }

    [Test]
    public async Task should_bind_async_on_task_when_success()
    {
        var x = await Task.FromResult(Result<string>.Ok("1"))
            .BindAsync(async s => await SafeTryParseAsync(s));
        Assert.That(x.GetValueOrDefault(), Is.EqualTo(1));
        Assert.That(x.Error, Is.Null);
    }

    [Test]
    public async Task should_bind_async_on_task_when_error()
    {
        var x = await Task.FromResult(Result<string>.Err(MyTestError.Create("err1", "error")))
            .BindAsync(async s => await SafeTryParseAsync(s));
        Assert.That(x.GetValueOrDefault(), Is.EqualTo(null));
        Assert.That(x.Error!.Key, Is.EqualTo("err1"));
    }

    [Test]
    public async Task should_bind_async_from_sync_when_success()
    {
        var x = await Result<string>.Ok("1")
            .BindAsync(async s => await SafeTryParseAsync(s));
        Assert.That(x.GetValueOrDefault(), Is.EqualTo(1));
        Assert.That(x.Error, Is.Null);
    }

    [Test]
    public async Task should_bind_async_from_sync_when_error()
    {
        var x = await Result<string>.Err(new Error("err1", "error"))
            .BindAsync(async s => await SafeTryParseAsync(s));
        Assert.That(x.GetValueOrDefault(), Is.Null);
        Assert.That(x.Error!.Key, Is.EqualTo("err1"));
    }

    private static Result<int> SafeTryParse(string value)
    {
        var parseSucceded = int.TryParse(value, out var result);
        if (parseSucceded) return result;
        return MyTestError.Create("parseIntError", $"{value} is not an int");
    }

    private static Task<Result<int?>> SafeTryParseAsync(string value)
    {
        var parseSucceded = int.TryParse(value, out var result);
        if (parseSucceded) return Task.FromResult(Result<int?>.Ok(result));
        return Task.FromResult(Result<int?>.Err(MyTestError.Create("parseIntError", $"{value} is not an int")));
    }
}
