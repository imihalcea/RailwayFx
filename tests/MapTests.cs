namespace RailwayFx.Tests;

public class MapTests
{
    [Test]
    public void should_map_success_result()
    {
        var x = Result<string>.Ok("a")
            .Map(s => s.ToUpper());
        Assert.That(x.GetValueOrDefault(), Is.EqualTo("A"));
    }

    [Test]
    public void should_map_error_result()
    {
        var x = Result<string>.Err(MyTestError.Create("err1", "error"))
            .Map(s => s.ToUpper());
        Assert.That(x.GetValueOrDefault(), Is.Null);
        Assert.That(x.Error!.Key, Is.EqualTo("err1"));
    }

    [Test]
    public async Task should_map_async_on_task_when_success()
    {
        var x = await Task.FromResult(Result<string>.Ok("a"))
            .MapAsync(async s => { await Task.Delay(0); return s.ToUpper(); });
        Assert.That(x.GetValueOrDefault(), Is.EqualTo("A"));
    }

    [Test]
    public async Task should_map_async_on_task_when_error()
    {
        var x = await Task.FromResult(Result<string>.Err(MyTestError.Create("err1", "error")))
            .MapAsync(async s => { await Task.Delay(0); return s.ToUpper(); });
        Assert.That(x.GetValueOrDefault(), Is.Null);
        Assert.That(x.Error!.Key, Is.EqualTo("err1"));
    }

    [Test]
    public async Task should_map_async_from_sync_when_success()
    {
        var x = await Result<string>.Ok("a")
            .MapAsync(async s => { await Task.Delay(0); return s.ToUpper(); });
        Assert.That(x.GetValueOrDefault(), Is.EqualTo("A"));
    }

    [Test]
    public async Task should_map_async_from_sync_when_error()
    {
        var x = await Result<string>.Err(new Error("err1", "error"))
            .MapAsync(async s => { await Task.Delay(0); return s.ToUpper(); });
        Assert.That(x.GetValueOrDefault(), Is.Null);
        Assert.That(x.Error!.Key, Is.EqualTo("err1"));
    }
}
