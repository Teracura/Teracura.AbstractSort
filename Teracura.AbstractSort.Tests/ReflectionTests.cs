namespace Teracura.AbstractSort.Tests;

using System.Collections.Generic;
using Shouldly;
using Logic.Configurations;
using Logic.Sorting;

#region Test Models

public class PublicOnly(string name, int age)
{
    public string Name { get; } = name;
    public readonly int Age = age;
}

public class InternalOnly(int v1, int v2)
{
    internal int InternalProp { get; } = v1;
    internal readonly int InternalField = v2;
}

public class BaseWithProtected(int @protected, int protectedField, int protectedInternal, int protectedPrivate)
{
    protected int ProtectedProp { get; init; } = @protected;
    protected int ProtectedField = protectedField;

    protected internal int ProtectedInternalProp { get; init; } = protectedInternal;
    private protected int PrivateProtectedProp { get; init; } = protectedPrivate;
}

public sealed class DerivedForProtected(
    string tag,
    int @protected,
    int protectedField,
    int protectedInternal,
    int protectedPrivate)
    : BaseWithProtected(@protected, protectedField, protectedInternal, protectedPrivate)
{
    public string Tag { get; } = tag;
}

public class WithPrivateMembers(string name, int secret)
{
    public string Name { get; } = name;

    private int _secretField = secret;
    private int PrivateProp { get; } = secret * 2;
}

public class WithNested(string label, InternalOnly inner)
{
    public string Label { get; } = label;
    public InternalOnly Inner { get; } = inner;
}

public class WithIndexer(params string[] items)
{
    private readonly List<string> _items = [..items];

    public string this[int i] => _items[i];
}

#endregion

internal static class TestHelpers
{
    public static List<T> SortByPath<T>(IEnumerable<T> source, string path, bool allowPrivate = false,
        bool ascending = true)
    {
        var cfg = new SortConfig<T>.Builder()
            .SortBy(path)
            .Ascending(ascending)
            .AllowPrivateAccess(allowPrivate)
            .Build();

        return source.Sort(cfg).ToList();
    }
}

public class ReflectionPublicMemberTests
{
    [Fact]
    public void Public_Property_Sorts_Ascending_By_Default()
    {
        var data = new[]
        {
            new PublicOnly("c", 30),
            new PublicOnly("a", 10),
            new PublicOnly("b", 20)
        };

        var sorted = TestHelpers.SortByPath(data, "Name");
        sorted.Select(x => x.Name).ShouldBe(["a", "b", "c"]);
    }

    [Fact]
    public void Public_Field_Sorts_Descending()
    {
        var data = new[]
        {
            new PublicOnly("x", 3),
            new PublicOnly("y", 1),
            new PublicOnly("z", 2)
        };

        var sorted = TestHelpers.SortByPath(data, "Age", ascending: false);
        sorted.Select(x => x.Age).ShouldBe([3, 2, 1]);
    }
}

public class ReflectionInternalAndProtectedTests
{
    [Fact]
    public void Internal_Property_Fails_Without_PrivateAccess()
    {
        var data = new[]
        {
            new InternalOnly(2, 9),
            new InternalOnly(1, 8),
            new InternalOnly(3, 7)
        };

        Should.Throw<ArgumentException>(() => TestHelpers.SortByPath(data, "InternalProp", allowPrivate: false));
    }

    [Fact]
    public void Internal_Property_Succeeds_With_PrivateAccess()
    {
        var data = new[]
        {
            new InternalOnly(2, 9),
            new InternalOnly(1, 8),
            new InternalOnly(3, 7)
        };

        var sorted = TestHelpers.SortByPath(data, "InternalProp", allowPrivate: true);
        sorted.Select(x => x.InternalProp).ShouldBe([1, 2, 3]);
    }

    [Fact]
    public void Internal_Field_Succeeds_With_PrivateAccess()
    {
        var data = new[]
        {
            new InternalOnly(10, 5),
            new InternalOnly(10, 2),
            new InternalOnly(10, 7)
        };

        var sorted = TestHelpers.SortByPath(data, "InternalField", allowPrivate: true);
        sorted.Select(x => x.InternalField).ShouldBe([2, 5, 7]);
    }

    [Fact]
    public void Protected_Property_Fails_Without_PrivateAccess()
    {
        var data = new[]
        {
            new DerivedForProtected("a", @protected: 3, protectedField: 30, protectedInternal: 300,
                protectedPrivate: 3000),
            new DerivedForProtected("b", @protected: 1, protectedField: 10, protectedInternal: 100,
                protectedPrivate: 1000),
            new DerivedForProtected("c", @protected: 2, protectedField: 20, protectedInternal: 200,
                protectedPrivate: 2000)
        };

        Should.Throw<ArgumentException>(() => TestHelpers.SortByPath(data, "ProtectedProp", allowPrivate: false));
    }

    [Fact]
    public void Protected_Property_Succeeds_With_PrivateAccess()
    {
        var data = new[]
        {
            new DerivedForProtected("a", 3, 30, 300, 3000),
            new DerivedForProtected("b", 1, 10, 100, 1000),
            new DerivedForProtected("c", 2, 20, 200, 2000)
        };

        var sorted = TestHelpers.SortByPath(data, "ProtectedProp", allowPrivate: true);
        sorted.Select(x => x.Tag).ShouldBe(["b", "c", "a"]);
    }

    [Fact]
    public void Protected_Field_Succeeds_With_PrivateAccess()
    {
        var data = new[]
        {
            new DerivedForProtected("x", 0, protectedField: 3, 0, 0),
            new DerivedForProtected("y", 0, protectedField: 1, 0, 0),
            new DerivedForProtected("z", 0, protectedField: 2, 0, 0)
        };

        var sorted = TestHelpers.SortByPath(data, "ProtectedField", allowPrivate: true);
        sorted.Select(x => x.Tag).ShouldBe(["y", "z", "x"]);
    }

    [Fact]
    public void ProtectedInternal_Property_Succeeds_With_PrivateAccess()
    {
        var data = new[]
        {
            new DerivedForProtected("m", 0, 0, protectedInternal: 20, 0),
            new DerivedForProtected("n", 0, 0, protectedInternal: 10, 0),
            new DerivedForProtected("o", 0, 0, protectedInternal: 30, 0)
        };

        var sorted = TestHelpers.SortByPath(data, "ProtectedInternalProp", allowPrivate: true);
        sorted.Select(x => x.Tag).ShouldBe(["n", "m", "o"]);
    }

    [Fact]
    public void PrivateProtected_Property_Succeeds_With_PrivateAccess()
    {
        var data = new[]
        {
            new DerivedForProtected("p", 0, 0, 0, protectedPrivate: 2),
            new DerivedForProtected("q", 0, 0, 0, protectedPrivate: 1),
            new DerivedForProtected("r", 0, 0, 0, protectedPrivate: 3)
        };

        var sorted = TestHelpers.SortByPath(data, "PrivateProtectedProp", allowPrivate: true);
        sorted.Select(x => x.Tag).ShouldBe(["q", "p", "r"]);
    }
}

public class ReflectionPrivateDefaultTests
{
    [Fact]
    public void Private_Field_Fails_Without_PrivateAccess()
    {
        var data = new[]
        {
            new WithPrivateMembers("a", 3),
            new WithPrivateMembers("b", 1),
            new WithPrivateMembers("c", 2),
        };

        Should.Throw<ArgumentException>(() => TestHelpers.SortByPath(data, "_secretField", allowPrivate: false));
    }

    [Fact]
    public void Private_Field_Succeeds_With_PrivateAccess()
    {
        var data = new[]
        {
            new WithPrivateMembers("a", 3),
            new WithPrivateMembers("b", 1),
            new WithPrivateMembers("c", 2),
        };

        var sorted = TestHelpers.SortByPath(data, "_secretField", allowPrivate: true);
        sorted.Select(x => x.Name).ShouldBe(["b", "c", "a"]);
    }

    [Fact]
    public void Private_Property_Succeeds_With_PrivateAccess()
    {
        var data = new[]
        {
            new WithPrivateMembers("u", 5),
            new WithPrivateMembers("v", 2),
            new WithPrivateMembers("w", 3),
        };

        var sorted = TestHelpers.SortByPath(data, "PrivateProp", allowPrivate: true);
        sorted.Select(x => x.Name).ShouldBe(["v", "w", "u"]);
    }
}

public class ReflectionNestedPathTests
{
    [Fact]
    public void Nested_Internal_Property_Fails_Without_PrivateAccess()
    {
        var data = new[]
        {
            new WithNested("A", new InternalOnly(3, 30)),
            new WithNested("B", new InternalOnly(1, 10)),
            new WithNested("C", new InternalOnly(2, 20)),
        };

        Should.Throw<ArgumentException>(() => TestHelpers.SortByPath(data, "Inner.InternalProp", allowPrivate: false));
    }

    [Fact]
    public void Nested_Internal_Property_Succeeds_With_PrivateAccess()
    {
        var data = new[]
        {
            new WithNested("A", new InternalOnly(3, 30)),
            new WithNested("B", new InternalOnly(1, 10)),
            new WithNested("C", new InternalOnly(2, 20)),
        };

        var sorted = TestHelpers.SortByPath(data, "Inner.InternalProp", allowPrivate: true);
        sorted.Select(x => x.Label).ShouldBe(["B", "C", "A"]);
    }
}

public class ReflectionIndexerTests
{
    [Fact]
    public void Public_Indexer_By_Int_Works()
    {
        List<WithIndexer> data =
        [
            new WithIndexer("z", "b", "c"),
            new WithIndexer("y", "c", "a"),
            new WithIndexer("x", "a", "b")
        ];

        var sorted = TestHelpers.SortByPath(data, "1", allowPrivate: false);
        sorted.First()[1].ShouldBe("a");
    }
}