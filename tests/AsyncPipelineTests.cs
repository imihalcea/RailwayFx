namespace RailwayFx.Tests;

public class AsyncPipelineTests
{
    [Test]
    public async Task should_chain_sync_result_into_full_async_pipeline()
    {
        var tapped = false;
        var x = await Result<string>.Ok("a")
            .MapAsync(async s => { await Task.Delay(0); return s.ToUpper(); })
            .TapAsync(async _ => { await Task.CompletedTask; tapped = true; })
            .BindAsync(s => Task.FromResult(Result<string>.Ok(s)));

        Assert.That(tapped, Is.True);
        Assert.That(x.GetValueOrDefault(), Is.EqualTo("A"));
    }

    [Test]
    public async Task should_propagate_cancellation_through_map_async_closure()
    {
        var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        var result = Task.FromResult(Result<string>.Ok("a"));

        Assert.ThrowsAsync<TaskCanceledException>(async () =>
            await result.MapAsync(async s =>
            {
                await Task.Delay(100, cts.Token);
                return s.ToUpper();
            }));
    }

    [Test]
    public async Task should_propagate_cancellation_through_bind_async_closure()
    {
        var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        var result = Task.FromResult(Result<string>.Ok("a"));

        Assert.ThrowsAsync<TaskCanceledException>(async () =>
            await result.BindAsync(async s =>
            {
                await Task.Delay(100, cts.Token);
                return Result<string>.Ok(s.ToUpper());
            }));
    }

    [Test]
    public async Task should_propagate_cancellation_through_tap_async_closure()
    {
        var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        var result = Task.FromResult(Result<string>.Ok("a"));

        Assert.ThrowsAsync<TaskCanceledException>(async () =>
            await result.TapAsync(async _ =>
            {
                await Task.Delay(100, cts.Token);
            }));
    }

    [Test]
    public async Task should_cancel_pipeline_at_first_step_that_observes_token()
    {
        var cts = new CancellationTokenSource();
        var step1Executed = false;
        var step2Executed = false;

        Assert.ThrowsAsync<TaskCanceledException>(async () =>
            await Task.FromResult(Result<string>.Ok("a"))
                .MapAsync(async s =>
                {
                    step1Executed = true;
                    await cts.CancelAsync();
                    return s.ToUpper();
                })
                .BindAsync(async s =>
                {
                    await Task.Delay(100, cts.Token);
                    step2Executed = true;
                    return Result<string>.Ok(s);
                }));

        Assert.That(step1Executed, Is.True);
        Assert.That(step2Executed, Is.False);
    }

    [Test]
    public async Task should_skip_callbacks_on_error_even_with_active_token()
    {
        var cts = new CancellationTokenSource();
        var callbackExecuted = false;

        var x = await Task.FromResult(Result<string>.Err(MyTestError.Create("err1", "error")))
            .MapAsync(async s =>
            {
                callbackExecuted = true;
                await Task.Delay(100, cts.Token);
                return s.ToUpper();
            });

        Assert.That(callbackExecuted, Is.False);
        Assert.That(x.Error!.Key, Is.EqualTo("err1"));
    }
}
