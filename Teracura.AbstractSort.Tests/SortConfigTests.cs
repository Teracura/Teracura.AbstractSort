using Shouldly;
using Teracura.AbstractSort.Logic;
using Teracura.AbstractSort.Logic.Configurations;

namespace Teracura.AbstractSort.Tests;

public class SortConfigTests
{
    // <> value of config won't matter for the following tests
    [Fact]
    public void Should_Default_Values()
    {
        var config = new SortConfig<int>.Builder().Build();
        config.ReflectionPaths.ShouldBe([]);
        config.SortingMethod.ShouldBe(SortingMethods.Reflection);
        config.Ascending.ShouldBeTrue();
    }

    [Fact]
    public void Should_Be_Descending()
    {
        var config = new SortConfig<int>.Builder().SortAscending(false).Build();
        config.Ascending.ShouldBeFalse();
    }

    [Fact]
    public void Should_Use_Property_Path()
    {
        var config = new SortConfig<int>.Builder().Build();
        config.SortingMethod.ShouldBe(SortingMethods.Reflection);
    }

    [Fact]
    public void Should_Use_Property_Path_With_Path()
    {
        const string path = "Name";
        var config = new SortConfig<int>.Builder().SortBy(path).Build();
        config.SortingMethod.ShouldBe(SortingMethods.Reflection);
        config.ReflectionPaths.ShouldBe([path]);
    }

    [Fact]
    public void Should_Use_Property_Path_With_Path_And_Descending()
    {
        const string path = "Name";
        var config = new SortConfig<int>.Builder().SortBy(path).SortAscending(false).Build();
        config.SortingMethod.ShouldBe(SortingMethods.Reflection);
        config.ReflectionPaths.ShouldBe([path]);
        config.Ascending.ShouldBeFalse();
        config.ReturnType.ShouldBe(ReturnType.List); //default;
    }

    [Fact]
    public void Should_Use_Property_Path_With_Path_And_ReturnType()
    {
        const string path = "Name";
        var config = new SortConfig<int>.Builder().SortBy(path).ReturnType(ReturnType.Queue)
            .Build();
        config.SortingMethod.ShouldBe(SortingMethods.Reflection);
        config.ReflectionPaths.ShouldBe([path]);
        config.ReturnType.ShouldBe(ReturnType.Queue);
        config.Ascending.ShouldBeTrue();
    }

    [Fact]
    public void Should_Use_Property_Path_With_Path_And_ReturnType_And_Descending()
    {
        const string path = "Name";
        var config = new SortConfig<int>.Builder().SortBy(path).ReturnType(ReturnType.Queue)
            .SortAscending(false).Build();
        config.SortingMethod.ShouldBe(SortingMethods.Reflection);
        config.ReflectionPaths.ShouldBe([path]);
        config.ReturnType.ShouldBe(ReturnType.Queue);
        config.Ascending.ShouldBeFalse();
    }

    [Fact]
    public void Should_Use_Property_Expression()
    {
        var config = new SortConfig<int>.Builder().SortBy(x => x).Build();
        config.SortingMethod.ShouldBe(SortingMethods.Lambda);
    }

    [Fact]
    public void Should_Use_Property_Expression_With_Path()
    {
        var config = new SortConfig<int>.Builder().SortBy("Name").SortBy(x => x).Build();
        config.SortingMethod.ShouldBe(SortingMethods.Lambda);
        config.ReflectionPaths.ShouldBe(["Name"]);
    }

    [Fact]
    public void Should_Use_Property_Expression_With_Path_And_Descending()
    {
        var config = new SortConfig<int>.Builder().SortBy("Name").SortBy(x => x)
            .SortAscending(false).Build();
        config.SortingMethod.ShouldBe(SortingMethods.Lambda);
        config.ReflectionPaths.ShouldBe(["Name"]);
        config.Ascending.ShouldBeFalse();
        config.ReturnType.ShouldBe(ReturnType.List); //default;
    }

    [Fact]
    public void Should_Use_Property_Expression_With_Path_And_ReturnType()
    {
        var config = new SortConfig<int>.Builder().SortBy("Name").SortBy(x => x)
            .ReturnType(ReturnType.Queue).Build();
        config.SortingMethod.ShouldBe(SortingMethods.Lambda);
        config.ReflectionPaths.ShouldBe(["Name"]);
        config.ReturnType.ShouldBe(ReturnType.Queue);
        config.Ascending.ShouldBeTrue();
    }
}