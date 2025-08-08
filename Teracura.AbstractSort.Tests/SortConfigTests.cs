using Shouldly;
using Teracura.AbstractSort.Logic;

namespace Teracura.AbstractSort.Tests;

public class SortConfigTests
{
    // <> value of config won't matter for the following tests
    [Fact]
    public void Should_Default_Values()
    {
        var config = new SortConfig<int>.Builder().Build();
        config.Path.ShouldBe("");
        config.UseReflectionPath.ShouldBeTrue();
        config.UsePropertyExpression.ShouldBeFalse();
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
        config.UseReflectionPath.ShouldBeTrue();
    }

    [Fact]
    public void Should_Use_Property_Path_With_Path()
    {
        const string path = "Name";
        var config = new SortConfig<int>.Builder().SetPropertyPath(path).Build();
        config.UseReflectionPath.ShouldBeTrue();
        config.UsePropertyExpression.ShouldBeFalse();
        config.Path.ShouldBe(path);
    }

    [Fact]
    public void Should_Use_Property_Path_With_Path_And_Descending()
    {
        const string path = "Name";
        var config = new SortConfig<int>.Builder().SetPropertyPath(path).SortAscending(false).Build();
        config.UseReflectionPath.ShouldBeTrue();
        config.UsePropertyExpression.ShouldBeFalse();
        config.Path.ShouldBe(path);
        config.Ascending.ShouldBeFalse();
        config.ReturnType.ShouldBe(ReturnType.List); //default;
    }

    [Fact]
    public void Should_Use_Property_Path_With_Path_And_ReturnType()
    {
        const string path = "Name";
        var config = new SortConfig<int>.Builder().SetPropertyPath(path).ReturnType(ReturnType.Queue)
            .Build();
        config.UseReflectionPath.ShouldBeTrue();
        config.UsePropertyExpression.ShouldBeFalse();
        config.Path.ShouldBe(path);
        config.ReturnType.ShouldBe(ReturnType.Queue);
        config.Ascending.ShouldBeTrue();
    }

    [Fact]
    public void Should_Use_Property_Path_With_Path_And_ReturnType_And_Descending()
    {
        const string path = "Name";
        var config = new SortConfig<int>.Builder().SetPropertyPath(path).ReturnType(ReturnType.Queue)
            .SortAscending(false).Build();
        config.UseReflectionPath.ShouldBeTrue();
        config.UsePropertyExpression.ShouldBeFalse();
        config.Path.ShouldBe(path);
        config.ReturnType.ShouldBe(ReturnType.Queue);
        config.Ascending.ShouldBeFalse();
    }

    [Fact]
    public void Should_Use_Property_Expression()
    {
        var config = new SortConfig<int>.Builder().UsePropertyLambda(x => x).Build();
        config.UseReflectionPath.ShouldBeFalse();
        config.UsePropertyExpression.ShouldBeTrue();
    }

    [Fact]
    public void Should_Use_Property_Expression_With_Path()
    {
        var config = new SortConfig<int>.Builder().SetPropertyPath("Name").UsePropertyLambda(x => x).Build();
        config.UseReflectionPath.ShouldBeFalse();
        config.UsePropertyExpression.ShouldBeTrue();
        config.Path.ShouldBe("Name");
    }

    [Fact]
    public void Should_Use_Property_Expression_With_Path_And_Descending()
    {
        var config = new SortConfig<int>.Builder().SetPropertyPath("Name").UsePropertyLambda(x => x)
            .SortAscending(false).Build();
        config.UseReflectionPath.ShouldBeFalse();
        config.UsePropertyExpression.ShouldBeTrue();
        config.Path.ShouldBe("Name");
        config.Ascending.ShouldBeFalse();
        config.ReturnType.ShouldBe(ReturnType.List); //default;
    }
    
    [Fact]
    public void Should_Use_Property_Expression_With_Path_And_ReturnType()
    {
        var config = new SortConfig<int>.Builder().SetPropertyPath("Name").UsePropertyLambda(x => x)
            .ReturnType(ReturnType.Queue).Build();
        config.UseReflectionPath.ShouldBeFalse();
        config.UsePropertyExpression.ShouldBeTrue();
        config.Path.ShouldBe("Name");
        config.ReturnType.ShouldBe(ReturnType.Queue);
        config.Ascending.ShouldBeTrue();
    }
}