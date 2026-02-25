namespace RailwayFx.Tests;

public record MyTestError : Error
{
    public static MyTestError Create(string key, string message) => new(key, message);
    private MyTestError(string key, string message) : base(key, message) { }
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
