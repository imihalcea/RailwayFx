namespace RailwayFx.Tests;

public class CollectionTests
{
    [Test]
    public void values_should_return_success_values()
    {
        var results = new[]
        {
            Result<string>.Ok("x"),
            Result<string>.Err(MyTestError.Create("a", "a")),
            Result<string>.Ok("y"),
            Result<string>.Err(MyTestError.Create("b", "b"))
        };

        Assert.That(results.Values(), Is.EquivalentTo(new[] { "x", "y" }));
    }

    [Test]
    public void errors_should_return_error_values()
    {
        var results = new[]
        {
            Result<string>.Ok("x"),
            Result<string>.Err(MyTestError.Create("a", "a")),
            Result<string>.Ok("y"),
            Result<string>.Err(MyTestError.Create("b", "b"))
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
            Result<string>.Ok("x"),
            Result<string>.Ok("y"),
            Result<string>.Err(MyTestError.Create("a", "a")),
            Result<string>.Err(MyTestError.Create("b", "b"))
        };

        var (errs, values) = results.SeparateResults();
        Assert.That(errs, Is.EquivalentTo(new[]
        {
            MyTestError.Create("a", "a"),
            MyTestError.Create("b", "b")
        }));

        Assert.That(values, Is.EquivalentTo(new[] { "x", "y" }));
    }
}
