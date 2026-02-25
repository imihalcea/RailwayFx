namespace RailwayFx.Tests;

public class TapTests
{
    [Test]
    public void should_tap_success_only_when_success()
    {
        var tapped = false;
        var result = Result<string>.Ok("a");
        var resultAfterTap = result.Tap(_ => tapped = true);

        Assert.That(tapped, Is.True);
        Assert.That(result, Is.EqualTo(resultAfterTap));
    }

    [Test]
    public void should_tap_success_only_when_error()
    {
        var tapped = false;
        var result = Result<string>.Err(MyTestError.Create("err1", "error"));
        var resultAfterTap = result.Tap(_ => tapped = true);

        Assert.That(tapped, Is.False);
        Assert.That(result, Is.EqualTo(resultAfterTap));
    }

    [Test]
    public void should_tap_error_callback_when_error()
    {
        var errorTaped = false;
        var successTaped = false;
        var result = Result<string>.Err(MyTestError.Create("err1", "error"));
        var resultAfterTap = result.Tap(_ => errorTaped = true, _ => successTaped = true);

        Assert.That(errorTaped, Is.True);
        Assert.That(successTaped, Is.False);
        Assert.That(result, Is.EqualTo(resultAfterTap));
    }

    [Test]
    public void should_tap_success_callback_when_success()
    {
        var errorTaped = false;
        var successTaped = false;
        var result = Result<string>.Ok("_");
        var resultAfterTap = result.Tap(_ => errorTaped = true, _ => successTaped = true);

        Assert.That(errorTaped, Is.False);
        Assert.That(successTaped, Is.True);
        Assert.That(result, Is.EqualTo(resultAfterTap));
    }

    [Test]
    public async Task should_tap_async_on_task_when_success()
    {
        var tapped = false;
        var x = await Task.FromResult(Result<string>.Ok("a"))
            .MapAsync(async s => { await Task.Delay(0); return s.ToUpper(); })
            .TapAsync(async _ => { await Task.CompletedTask; tapped = true; });

        Assert.That(tapped, Is.True);
        Assert.That(x.GetValueOrDefault(), Is.EqualTo("A"));
    }

    [Test]
    public async Task should_not_tap_async_on_task_when_error()
    {
        var tapped = false;
        var x = await Task.FromResult(Result<string>.Err(MyTestError.Create("err1", "error")))
            .TapAsync(async _ => { await Task.CompletedTask; tapped = true; });

        Assert.That(tapped, Is.False);
        Assert.That(x.Error!.Key, Is.EqualTo("err1"));
    }

    [Test]
    public async Task should_tap_async_both_on_task_when_success()
    {
        var errorTapped = false;
        var successTapped = false;
        var x = await Task.FromResult(Result<string>.Ok("a"))
            .TapAsync(
                async _ => { await Task.CompletedTask; errorTapped = true; },
                async _ => { await Task.CompletedTask; successTapped = true; });

        Assert.That(errorTapped, Is.False);
        Assert.That(successTapped, Is.True);
        Assert.That(x.GetValueOrDefault(), Is.EqualTo("a"));
    }

    [Test]
    public async Task should_tap_async_both_on_task_when_error()
    {
        var errorTapped = false;
        var successTapped = false;
        var x = await Task.FromResult(Result<string>.Err(MyTestError.Create("err1", "error")))
            .TapAsync(
                async _ => { await Task.CompletedTask; errorTapped = true; },
                async _ => { await Task.CompletedTask; successTapped = true; });

        Assert.That(errorTapped, Is.True);
        Assert.That(successTapped, Is.False);
        Assert.That(x.Error!.Key, Is.EqualTo("err1"));
    }

    [Test]
    public async Task should_tap_async_from_sync_when_success()
    {
        var tapped = false;
        var result = Result<string>.Ok("a");
        var x = await result.TapAsync(async _ => { await Task.CompletedTask; tapped = true; });

        Assert.That(tapped, Is.True);
        Assert.That(x.GetValueOrDefault(), Is.EqualTo("a"));
    }

    [Test]
    public async Task should_tap_async_from_sync_when_error()
    {
        var tapped = false;
        var result = Result<string>.Err(new Error("err1", "error"));
        var x = await result.TapAsync(async _ => { await Task.CompletedTask; tapped = true; });

        Assert.That(tapped, Is.False);
        Assert.That(x.Error!.Key, Is.EqualTo("err1"));
    }

    [Test]
    public async Task should_tap_async_both_from_sync_when_success()
    {
        var errorTapped = false;
        var successTapped = false;
        var result = Result<string>.Ok("a");
        var x = await result.TapAsync(
            async _ => { await Task.CompletedTask; errorTapped = true; },
            async _ => { await Task.CompletedTask; successTapped = true; });

        Assert.That(errorTapped, Is.False);
        Assert.That(successTapped, Is.True);
        Assert.That(x.GetValueOrDefault(), Is.EqualTo("a"));
    }

    [Test]
    public async Task should_tap_async_both_from_sync_when_error()
    {
        var errorTapped = false;
        var successTapped = false;
        var result = Result<string>.Err(new Error("err1", "error"));
        var x = await result.TapAsync(
            async _ => { await Task.CompletedTask; errorTapped = true; },
            async _ => { await Task.CompletedTask; successTapped = true; });

        Assert.That(errorTapped, Is.True);
        Assert.That(successTapped, Is.False);
        Assert.That(x.Error!.Key, Is.EqualTo("err1"));
    }
}
