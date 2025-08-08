using Shouldly;
using Teracura.AbstractSort.Logic;

namespace Teracura.AbstractSort.Tests;

public class SortConfigTests
{
    [Fact]
    public void Should_Default_Values()
    {
        var config = new SortConfigTest.Builder().Build();
        config.Path.ShouldBe("");
        config.UsePath.ShouldBeFalse();
        config.Ascending.ShouldBeTrue();
    }

    [Fact]
    public void Should_Be_Descending()
    {
        var config = new SortConfigTest.Builder().SortAscending(false).Build();
        config.Ascending.ShouldBeFalse();
    }

    [Fact]
    public void Should_Use_Property_Path()
    {
        var config = new SortConfigTest.Builder().UsePropertyPath().Build();
        config.UsePath.ShouldBeTrue();
    }

    [Fact]
    public void Should_Use_Property_Path_With_Path()
    {
        const string path = "Name";
        var config = new SortConfigTest.Builder().UsePropertyPath().SetPropertyPath(path).Build();
        config.UsePath.ShouldBeTrue();
        config.Path.ShouldBe(path);
    }

    [Fact]
    public void Should_Use_Property_Path_With_Path_And_Descending()
    {
        const string path = "Name";
        var config = new SortConfigTest.Builder().UsePropertyPath().SetPropertyPath(path).SortAscending(false).Build();
        config.UsePath.ShouldBeTrue();
        config.Path.ShouldBe(path);
        config.Ascending.ShouldBeFalse();
        config.ReturnType.ShouldBe(ReturnType.List); //default;
    }

    [Fact]
    public void Should_Use_Property_Path_With_Path_And_ReturnType()
    {
        const string path = "Name";
        var config = new SortConfigTest.Builder().UsePropertyPath().SetPropertyPath(path).ReturnType(ReturnType.Queue)
            .Build();
        config.UsePath.ShouldBeTrue();
        config.Path.ShouldBe(path);
        config.ReturnType.ShouldBe(ReturnType.Queue);
        config.Ascending.ShouldBeTrue();
    }

    [Fact]
    public void Should_Use_Property_Path_With_Path_And_ReturnType_And_Descending()
    {
        const string path = "Name";
        var config = new SortConfigTest.Builder().UsePropertyPath().SetPropertyPath(path).ReturnType(ReturnType.Queue)
            .SortAscending(false).Build();
        config.UsePath.ShouldBeTrue();
        config.Path.ShouldBe(path);
        config.ReturnType.ShouldBe(ReturnType.Queue);
        config.Ascending.ShouldBeFalse();
    }

    [Fact]
    public void Should_Sort_Config_Descending_Queue()
    {
        var config = new SortConfigTest.Builder().SortAscending(false).ReturnType(ReturnType.Queue).Build();
        config.Ascending.ShouldBeFalse();
        config.ReturnType.ShouldBe(ReturnType.Queue);
        List<int> list = [1, 2, 3, 4, 5, 6, 9, 8, 7, 10];
        var obj = list.SortLength(config);
        obj.ShouldBeOfType<Queue<int>>();
        list.ShouldBe([10, 9, 8, 7, 6, 5, 4, 3, 2, 1]);
    }

    [Fact]
    public void Should_Sort_Config_Descending_HashSet()
    {
        var config = new SortConfigTest.Builder().SortAscending(false).ReturnType(ReturnType.HashSet).Build();
        config.Ascending.ShouldBeFalse();
        config.ReturnType.ShouldBe(ReturnType.HashSet);
        List<int> list = [1, 2, 3, 4, 5, 6, 9, 8, 7, 10];
        var obj = list.SortLength(config);
        obj.ShouldBeOfType<HashSet<int>>();
        list.ShouldBe([10, 9, 8, 7, 6, 5, 4, 3, 2, 1]);
    }

    [Fact]
    public void Should_Sort_Config_Descending_Stack()
    {
        var config = new SortConfigTest.Builder().SortAscending(false).ReturnType(ReturnType.Stack).Build();
        config.Ascending.ShouldBeFalse();
        config.ReturnType.ShouldBe(ReturnType.Stack);
        List<int> list = [1, 2, 3, 4, 5, 6, 9, 8, 7, 10];
        var obj = list.SortLength(config);
        obj.ShouldBeOfType<Stack<int>>();
        list.ShouldBe([10, 9, 8, 7, 6, 5, 4, 3, 2, 1]);
    }
}

public class SortConfigTest
{
    public string Path { get; set; } = "";
    public bool UsePath { get; set; } = false;
    public bool Ascending { get; set; } = true;
    public ReturnType ReturnType { get; set; } = ReturnType.List;

    private SortConfigTest()
    {
    }

    public class Builder
    {
        private readonly SortConfigTest _config = new();

        public Builder UsePropertyPath(bool usePropertyPath = true)
        {
            _config.UsePath = usePropertyPath;
            return this;
        }

        public Builder SetPropertyPath(string path)
        {
            _config.Path = path;
            return this;
        }

        public Builder SortAscending(bool ascending = true)
        {
            _config.Ascending = ascending;
            return this;
        }

        public Builder ReturnType(ReturnType type = Logic.ReturnType.List)
        {
            _config.ReturnType = type;
            return this;
        }

        public SortConfigTest Build()
        {
            return _config;
        }
    }
}

public static class AbstractSorterTest
{
    public static object SortLength<T>(this List<T> list, SortConfigTest? config = null)
    {
        config ??= new SortConfigTest.Builder().Build();
        var reflectionPath = config.Path;
        var ascending = config.Ascending;
        var returnType = config.ReturnType;

        var sorted = string.IsNullOrEmpty(reflectionPath)
            ? list.OrderBy(item => item?.ToString()?.Length ?? -1)
                .ThenBy(item => item?.ToString())
                .ToList()
            : list.OrderBy(item =>
                    GetPropertyValue(item, reflectionPath)?.ToString()?.Length ?? -1)
                .ThenBy(item =>
                    GetPropertyValue(item, reflectionPath)?.ToString() ?? "")
                .ToList();
        if (!ascending) sorted.Reverse();

        list.Clear();
        list.AddRange(sorted);

        return ReturnFromType(returnType, sorted);
    }

    private static object? GetPropertyValue(object? obj, string propertyPath)
    {
        if (obj == null) return null;

        var parts = propertyPath.Split('.');
        var current = obj;

        foreach (var part in parts)
        {
            if (current == null) return null;

            var prop = current.GetType().GetProperty(part);
            if (prop == null) return null;

            current = prop.GetValue(current);
        }

        return current;
    }

    private static object ReturnFromType<T>(ReturnType returnType, List<T> sorted)
    {
        return returnType switch
        {
            ReturnType.List => sorted,
            ReturnType.Queue => new Queue<T>(sorted),
            ReturnType.Stack => new Stack<T>(sorted),
            ReturnType.HashSet => new HashSet<T>(sorted),
            _ => throw new ArgumentOutOfRangeException(nameof(returnType), $"Unknown return type: {returnType}")
        };
    }
}