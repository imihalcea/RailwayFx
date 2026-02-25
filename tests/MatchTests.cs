namespace RailwayFx.Tests;

public class MatchTests
{
    [Test]
    public void should_match_success_result()
    {
        var x = Result<string>.Ok("success")
            .Match(
                whenError: err => err.Message,
                whenSuccess: _ => "success");
        Assert.That(x, Is.EqualTo("success"));
    }

    [Test]
    public void should_match_error_result()
    {
        var x = Result<string>.Err(MyTestError.Create("err1", "error"))
            .Match(
                whenError: err => err.Message,
                whenSuccess: _ => "success");
        Assert.That(x, Is.EqualTo("error"));
    }

    [Test]
    public async Task should_match_async_on_task_when_success()
    {
        var x = await Task.FromResult(Result<string>.Ok("success"))
            .MatchAsync(
                whenError: err => Task.FromResult(err.Message),
                whenSuccess: _ => Task.FromResult("success"));
        Assert.That(x, Is.EqualTo("success"));
    }

    [Test]
    public async Task should_match_async_on_task_when_error()
    {
        var x = await Task.FromResult(Result<string>.Err(MyTestError.Create("err1", "error")))
            .MatchAsync(
                whenError: err => Task.FromResult(err.Message),
                whenSuccess: _ => Task.FromResult("success"));
        Assert.That(x, Is.EqualTo("error"));
    }

    [Test]
    public async Task should_match_async_on_instance_when_success()
    {
        var result = Result<string>.Ok("hello");
        var x = await result.MatchAsync(
            whenError: err => Task.FromResult(err.Message),
            whenSuccess: val => Task.FromResult(val.ToUpper()));
        Assert.That(x, Is.EqualTo("HELLO"));
    }

    [Test]
    public async Task should_match_async_on_instance_when_error()
    {
        var result = Result<string>.Err(new Error("err1", "error"));
        var x = await result.MatchAsync(
            whenError: err => Task.FromResult(err.Message),
            whenSuccess: val => Task.FromResult(val.ToUpper()));
        Assert.That(x, Is.EqualTo("error"));
    }
}
