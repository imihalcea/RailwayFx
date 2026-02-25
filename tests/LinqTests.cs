namespace RailwayFx.Tests;

public class LinqTests
{
    [Test]
    public void should_map_success_result_with_linq()
    {
        var x = from s in Result<string>.Ok("a")
            select s.ToUpper();
        Assert.That(x.GetValueOrDefault(), Is.EqualTo("A"));
    }

    [Test]
    public void should_map_error_result_with_linq()
    {
        var x = from s in Result<string>.Err(MyTestError.Create("err1", "error"))
            select s.ToUpper();
        Assert.That(x.GetValueOrDefault(), Is.Null);
        Assert.That(x.Error!.Key, Is.EqualTo("err1"));
    }

    [Test]
    public void should_combine_success_result_with_linq()
    {
        var x = from s1 in Result<string>.Ok("foo")
            from s2 in Result<string>.Ok("bar")
            select s1 + s2;
        Assert.That(x.GetValueOrDefault(), Is.EqualTo("foobar"));
        Assert.That(x.Error, Is.Null);
    }

    [Test]
    public void should_combine_error_result_with_linq()
    {
        var x = from s1 in Result<string>.Err(MyTestError.Create("err1", "error"))
            from s2 in Result<string>.Ok("bar")
            select s1 + s2;
        Assert.That(x.GetValueOrDefault(), Is.Null);
        Assert.That(x.Error!.Key, Is.EqualTo("err1"));
    }

    [Test]
    public void should_combine_other_error_result_with_linq()
    {
        var x = from s1 in Result<string>.Ok("foo")
            from s2 in Result<string>.Err(MyTestError.Create("err1", "error"))
            select s1 + s2;
        Assert.That(x.GetValueOrDefault(), Is.Null);
        Assert.That(x.Error!.Key, Is.EqualTo("err1"));
    }

    [Test]
    public void should_combine_multiple_error_result_with_linq()
    {
        var x = from s1 in Result<string>.Err(MyTestError.Create("err1", "error"))
            from s2 in Result<string>.Err(MyTestError.Create("err2", "error"))
            select s1 + s2;
        Assert.That(x.GetValueOrDefault(), Is.Null);
        Assert.That(x.Error!.Key, Is.EqualTo("err1"));
    }
}
