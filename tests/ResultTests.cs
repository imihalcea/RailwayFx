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
    public void should_match_error_result()
    {
        var x = ItReturnsErrorResult("err1", "error")
            .Match(
                whenError: err => err.Message, 
                whenSuccess: _ => "success");
        Assert.That(x, Is.EqualTo("error"));
    }

    [Test]
    public void should_match_success_result()
    {
        var x = ItReturnsSuccessResultOf("success")
            .Match(
                whenError: err => err.Message,
                whenSuccess: _ => "success"
            );
        Assert.That(x, Is.EqualTo("success"));
    }


    [Test]
    public void should_return_the_value()
    {
        const string defaultValue = "default value";
        var valueFromError = ItReturnsErrorResult("err1", "error").GetValueOrDefault(defaultValue);
        var valueFromSuccess = ItReturnsSuccessResultOf("my value").GetValueOrDefault(defaultValue);
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
        Assert.Throws<InvalidOperationException>(()=> r1.Unwrap());
    }

    [Test]
    public void unwrap_should_return_value_when_success()
    {
        var r1 = Result<string>.Ok("foo");
        Assert.That(r1.Unwrap(), Is.EqualTo("foo"));
    }

    [Test]
    public void ok_should_throw_when_value_is_null()
    {
        Assert.Throws<ArgumentNullException>(() => Result<string>.Ok(null!));
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
    public void implicit_conversion_from_null_value_should_throw()
    {
        Assert.Throws<ArgumentNullException>(() => { Result<string> _ = (string)null!; });
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

    private static Result<string> ItReturnsSuccessResultOf(string value)
    {
        return Result<string>.Ok(value);
    }

    private static Result<string> ItReturnsErrorResult(string errorKey, string errorMessage)
    {
        var error = MyTestError.Create(errorKey, errorMessage);
        return Result<string>.Err(error);
    }
}

public class SuperBizz : IEquatable<SuperBizz>
{
    public string A { get; }
    public int B { get; }

    public SuperBizz(string a, int b)
    {
        A = a;
        B = b;
    }

    public bool Equals(SuperBizz? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return A == other.A && B == other.B;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((SuperBizz)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(A, B);
    }
}

public class Bizz : SuperBizz, IEquatable<Bizz>
{
    public int C { get; }

    public Bizz(string a, int b, int c) : base(a, b)
    {
        C = c;
    }

    public bool Equals(Bizz? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return base.Equals(other) && C == other.C;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((Bizz)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(), C);
    }
}

public class BizzPrim : SuperBizz, IEquatable<BizzPrim>
{
    public int D { get; }

    public BizzPrim(string a, int b, int d) : base(a, b)
    {
        D = d;
    }

    public bool Equals(BizzPrim? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return base.Equals(other) && D == other.D;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((BizzPrim)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(), D);
    }
}

public class Qix : IEquatable<Qix>
{
    public string A { get; }
    public int B { get; }

    public Qix(string a, int b)
    {
        A = a;
        B = b;
    }

    public bool Equals(Qix? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return A == other.A && B == other.B;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((Qix)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(A, B);
    }
}

public record MyTestError : Error
{
    public static MyTestError Create(string key, string message)
    {
        return new MyTestError(key, message);
    }

    private MyTestError(string key, string message) : base(key, message)
    {
    }
}