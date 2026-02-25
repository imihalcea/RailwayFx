namespace RailwayFx.Tests;

[TestFixture]
public class ResultTests
{
    [Test]
    public void should_create_error_result()
    {
        var error = MyTestError.Create("err1", "error");
        var result = Result<string>.Err(error);
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.IsError, Is.True);
        Assert.That(result.Error, Is.EqualTo(error));
        Assert.That(result.Value, Is.Null);
    }

    [Test]
    public void should_create_value_result()
    {
        var result = Result<string>.Ok("success");
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.GetValueOrDefault(), Is.EqualTo("success"));
        Assert.That(result.Error, Is.Null);
    }

    [Test]
    public void should_return_the_value()
    {
        const string defaultValue = "default value";
        var valueFromError = Result<string>.Err(MyTestError.Create("err1", "error")).GetValueOrDefault(defaultValue);
        var valueFromSuccess = Result<string>.Ok("my value").GetValueOrDefault(defaultValue);
        Assert.That(valueFromError, Is.EqualTo(defaultValue));
        Assert.That(valueFromSuccess, Is.EqualTo("my value"));
    }

    [Test]
    public void should_be_equal_when_have_the_same_value()
    {
        var r1 = Result<string>.Ok("a");
        var r2 = Result<string>.Ok("a");
        Assert.That(r1, Is.EqualTo(r2));
    }

    [Test]
    public void should_not_be_equal_when_do_not_have_the_same_value()
    {
        var r1 = Result<string>.Ok("a");
        var r2 = Result<string>.Ok("b");
        Assert.That(r1, Is.Not.EqualTo(r2));
    }

    [Test]
    public void should_be_equal_when_have_the_same_error()
    {
        var r1 = Result<string>.Err(new Error("key", "message"));
        var r2 = Result<string>.Err(new Error("key", "message"));
        Assert.That(r1, Is.EqualTo(r2));
    }

    [Test]
    public void should_not_be_equal_when_dont_have_the_same_error()
    {
        var r1 = Result<string>.Err(new Error("key", "message"));
        var r2 = Result<string>.Err(new Error("key1", "message"));
        Assert.That(r1, Is.Not.EqualTo(r2));
    }

    [Test]
    public void should_be_equal_when_have_the_same_equatable_value()
    {
        var r1 = Result<Qix>.Ok(new Qix("A", 1));
        var r2 = Result<Qix>.Ok(new Qix("A", 1));
        Assert.That(r1, Is.EqualTo(r2));
    }

    [Test]
    public void should_not_be_equal_when_do_not_have_the_same_equatable_value()
    {
        var r1 = Result<Qix>.Ok(new Qix("A", 1));
        var r2 = Result<Qix>.Ok(new Qix("A", 2));
        Assert.That(r1, Is.Not.EqualTo(r2));
    }

    [Test]
    public void should_not_be_equal_error_and_success()
    {
        var r1 = Result<int>.Err(new Error("key", "message"));
        var r2 = Result<int>.Ok(0);
        Assert.That(r1, Is.Not.EqualTo(r2));
    }

    [Test]
    public void should_not_be_equal_inheritace()
    {
        var r1 = Result<SuperBizz>.Ok(new Bizz("A", 1, 2));
        var r2 = Result<SuperBizz>.Ok(new BizzPrim("A", 1, 2));
        Assert.That(r1, Is.Not.EqualTo(r2));
    }

    [Test]
    public void unwrap_should_throw_when_is_error()
    {
        var r1 = Result<string>.Err(new Error("key", "message"));
        Assert.Throws<InvalidOperationException>(() => r1.Unwrap());
    }

    [Test]
    public void unwrap_should_return_value_when_success()
    {
        var r1 = Result<string>.Ok("foo");
        Assert.That(r1.Unwrap(), Is.EqualTo("foo"));
    }

    [Test]
    public void ok_should_accept_null_for_nullable_types()
    {
        var result = Result<string?>.Ok(null);
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Is.Null);
    }

    [Test]
    public void err_should_throw_when_error_is_null()
    {
        Assert.Throws<ArgumentNullException>(() => Result<string>.Err(null!));
    }

    [Test]
    public void implicit_conversion_from_value_should_create_success()
    {
        Result<string> result = "hello";
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Is.EqualTo("hello"));
    }

    [Test]
    public void implicit_conversion_from_error_should_create_error()
    {
        Result<string> result = new Error("key", "msg");
        Assert.That(result.IsError, Is.True);
        Assert.That(result.Error!.Key, Is.EqualTo("key"));
    }

    [Test]
    public void implicit_conversion_from_null_value_should_create_success()
    {
        Result<string?> result = (string?)null;
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Is.Null);
    }

    [Test]
    public void equality_operator_should_return_true_for_equal_results()
    {
        var r1 = Result<string>.Ok("a");
        var r2 = Result<string>.Ok("a");
        Assert.That(r1 == r2, Is.True);
        Assert.That(r1 != r2, Is.False);
    }

    [Test]
    public void equality_operator_should_return_false_for_different_results()
    {
        var r1 = Result<string>.Ok("a");
        var r2 = Result<string>.Ok("b");
        Assert.That(r1 == r2, Is.False);
        Assert.That(r1 != r2, Is.True);
    }

    [Test]
    public void tostring_should_format_success()
    {
        var result = Result<string>.Ok("hello");
        Assert.That(result.ToString(), Is.EqualTo("Success[hello]"));
    }

    [Test]
    public void tostring_should_format_error()
    {
        var result = Result<string>.Err(new Error("key", "something failed"));
        Assert.That(result.ToString(), Is.EqualTo("Error[something failed]"));
    }
}
