namespace RailwayFx.Tests;

public class InteropTests
{
    [Test]
    public void when_no_exception_is_raised_returns_a_success_result()
    {
        var result = new Func<string>(() => "1" + "1").RInvoke(_ => new Error("unused", "unused"));
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.GetValueOrDefault(), Is.EqualTo("11"));
    }

    [Test]
    public void when_an_exception_is_raised_returns_an_error_result_and_do_not_rethrows_the_exception()
    {
        var result = new Func<int>(() => throw new Exception("boom")).RInvoke(e => new SomeError(e));
        Assert.That(result.IsError, Is.True);
        Assert.That(result.Error, Is.InstanceOf<SomeError>());
        Assert.That(result.Error.Message, Is.EqualTo("boom"));
    }

    [Test]
    public void when_error_throw_exception()
    {
        var r1 = Result<int>.Err(new Error("error", "msg"));
        Assert.Throws<InvalidOperationException>(() => r1.ThrowOnError());
    }

    [Test]
    public void when_success_return_result()
    {
        var r1 = Result<int>.Ok(42);
        var r2 = r1.ThrowOnError();
        Assert.That(ReferenceEquals(r1, r2), Is.True);
    }

    // ReSharper disable once NotAccessedPositionalProperty.Local
    private record SomeError(Exception E) : Error(E.GetType().Name, E.Message);
}
