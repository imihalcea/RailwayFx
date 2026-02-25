namespace RailwayFx.Tests;

public class ResultExtensionTests
{
    [Test]
    public void should_map_success_result()
    {
        var x = SuccessResultOf("a")
            .Map(s => s.ToUpper());
        Assert.That(x.GetValueOrDefault(), Is.EqualTo("A"));
    }

    [Test]
    public void should_map_error_result()
    {
        var x = ErrorResult("err1", "error")
            .Map(s => s.ToUpper());
        Assert.That(x.GetValueOrDefault(), Is.Null);
        Assert.That(x.Error!.Key, Is.EqualTo("err1"));
    }

    [Test]
    public void should_bind_error_result()
    {
        var x = ErrorResult("err1", "error")
            .Bind(s => SafeTryParse(s));
        Assert.That(x.GetValueOrDefault(), Is.EqualTo(0));
        Assert.That(x.Error!.Key, Is.EqualTo("err1"));
    }

    [Test]
    public void should_bind_success_result()
    {
        var x = SuccessResultOf("1")
            .Bind(s => SafeTryParse(s));
        Assert.That(x.GetValueOrDefault(), Is.EqualTo(1));
        Assert.That(x.Error, Is.Null);
    }


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

    [Test]
    public void should_tap_success_only_when_success()
    {
        var tapped = false;
        var result = SuccessResultOf("a");
        var resultAfterTap = result.Tap(_ => tapped = true);

        Assert.That(tapped, Is.True);
        Assert.That(result, Is.EqualTo(resultAfterTap));
    }

    [Test]
    public void should_tap_success_only_when_error()
    {
        var tapped = false;
        var result = ErrorResult("err1", "error");
        var resultAfterTap = result.Tap(_ => tapped = true);

        Assert.That(tapped, Is.False);
        Assert.That(result, Is.EqualTo(resultAfterTap));
    }

    [Test]
    public void should_tap_error()
    {
        var errorTaped = false;
        var successTaped = false;
        var errorResult = ErrorResult("err1", "error");
        var resultAfterTap =
            errorResult.Tap(_ => errorTaped = true, _ => successTaped = true);

        Assert.That(errorTaped, Is.True);
        Assert.That(successTaped, Is.False);
        Assert.That(errorResult, Is.EqualTo(resultAfterTap));
    }

    [Test]
    public void should_tap_success()
    {
        var errorTaped = false;
        var successTaped = false;
        var errorResult = SuccessResultOf("_");
        var resultAfterTap =
            errorResult.Tap(_ => errorTaped = true, _ => successTaped = true);

        Assert.That(errorTaped, Is.False);
        Assert.That(successTaped, Is.True);
        Assert.That(errorResult, Is.EqualTo(resultAfterTap));
    }


    [Test]
    public void should_map_success_result_with_linq()
    {
        var x = from s in SuccessResultOf("a")
            select s.ToUpper();
        Assert.That(x.GetValueOrDefault(), Is.EqualTo("A"));
    }

    [Test]
    public void should_map_error_result_with_linq()
    {
        var x = from s in ErrorResult("err1", "error")
            select s.ToUpper();
        Assert.That(x.GetValueOrDefault(), Is.Null);
        Assert.That(x.Error!.Key, Is.EqualTo("err1"));
    }

    [Test]
    public void should_combine_success_result_with_linq()
    {
        var x = from s1 in SuccessResultOf("foo")
            from s2 in SuccessResultOf("bar")
            select s1 + s2;
        Assert.That(x.GetValueOrDefault(), Is.EqualTo("foobar"));
        Assert.That(x.Error, Is.Null);
    }

    [Test]
    public void should_combine_error_result_with_linq()
    {
        var x = from s1 in ErrorResult("err1", "error")
            from s2 in SuccessResultOf("bar")
            select s1 + s2;
        Assert.That(x.GetValueOrDefault(), Is.Null);
        Assert.That(x.Error!.Key, Is.EqualTo("err1"));
    }

    [Test]
    public void should_combine_other_error_result_with_linq()
    {
        var x = from s1 in SuccessResultOf("foo")
            from s2 in ErrorResult("err1", "error")
            select s1 + s2;
        Assert.That(x.GetValueOrDefault(), Is.Null);
        Assert.That(x.Error!.Key, Is.EqualTo("err1"));
    }

    [Test]
    public void should_combine_multiple_error_result_with_linq()
    {
        var x = from s1 in ErrorResult("err1", "error")
            from s2 in ErrorResult("err2", "error")
            select s1 + s2;
        Assert.That(x.GetValueOrDefault(), Is.Null);
        Assert.That(x.Error!.Key, Is.EqualTo("err1"));
    }


    [Test]
    public void values_should_return_success_values()
    {
        var results = new[]
        {
            SuccessResultOf("x"),
            ErrorResult("a", "a"),
            SuccessResultOf("y"),
            ErrorResult("b", "b")
        };

        Assert.That(results.Values(), Is.EquivalentTo(new[] { "x", "y" }));
    }

    [Test]
    public void errors_should_return_error_values()
    {
        var results = new[]
        {
            SuccessResultOf("x"),
            ErrorResult("a", "a"),
            SuccessResultOf("y"),
            ErrorResult("b", "b")
        };

        Assert.That(results.Errors(), Is.EquivalentTo(new[]
        {
            MyTestError.Create("a", "a"),
            MyTestError.Create("b", "b")
        }));
    }

    [Test]
    public void separate_result()
    {
        var results = new[]
        {
            SuccessResultOf("x"),
            SuccessResultOf("y"),
            ErrorResult("a", "a"),
            ErrorResult("b", "b")
        };

        var (errs, values) = results.SeparateResults();
        Assert.That(errs, Is.EquivalentTo(new[]
        {
            MyTestError.Create("a", "a"),
            MyTestError.Create("b", "b")
        }));

        Assert.That(values, Is.EquivalentTo(new[]
        {
            "x",
            "y"
        }));
    }

    private static Result<string> SuccessResultOf(string value)
    {
        return Result<string>.Ok(value);
    }

    private static Result<string> ErrorResult(string errorKey, string errorMessage)
    {
        var error = MyTestError.Create(errorKey, errorMessage);
        return Result<string>.Err(error);
    }

    private Result<int> SafeTryParse(string value)
    {
        var parseSucceded = int.TryParse(value, out var result);
        if (parseSucceded) return result;

        return MyTestError.Create("parseIntError", $"{value} is not an int");
    }

    // ReSharper disable once NotAccessedPositionalProperty.Local
    private record SomeError(Exception E) : Error(E.GetType().Name, E.Message);
}