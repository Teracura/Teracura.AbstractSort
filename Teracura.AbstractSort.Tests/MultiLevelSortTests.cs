using Shouldly;
using Teracura.AbstractSort.Logic;
using Teracura.AbstractSort.Logic.Configurations;
using Teracura.AbstractSort.Logic.Sorting;
using Xunit;

namespace Teracura.AbstractSort.Tests;

public class MultiLevelSortTests
{
    private class Person
    {
        public string Name { get; set; } = "";
        public string Title { get; set; } = "";
        public int Age { get; set; }
    }


    [Fact]
    public void Should_sort_by_name_then_by_title_using_lambda()
    {
        var obj = new Person { Name = "Alice", Title = "Engineer", Age = 30 };
        var obj2 = new Person { Name = "Bob", Title = "Manager", Age = 25 };
        var obj3 = new Person { Name = "Charlie", Title = "Engineer", Age = 35 };
        var obj4 = new Person { Name = "Alice", Title = "Analyst", Age = 28 };
        List<Person> list = [obj, obj2, obj3, obj4];

        var config = new SortConfig<Person>.Builder()
            .SortBy(p => p.Name)
            .ThenBy(p => p.Title)
            .Build();

        var sorted = (List<Person>)list.SortLength(config);

        sorted.ShouldBe([obj2, obj4, obj, obj3]);
    }

    [Fact]
    public void Should_sort_by_title_then_by_age_using_reflection()
    {
        var obj = new Person { Name = "Alice", Title = "Engineer", Age = 30 };
        var obj2 = new Person { Name = "Bob", Title = "Manager", Age = 25 };
        var obj3 = new Person { Name = "Charlie", Title = "Engineer", Age = 35 };
        var obj4 = new Person { Name = "Alice", Title = "Analyst", Age = 28 };
        List<Person> list = [obj, obj2, obj3, obj4];

        var config = new SortConfig<Person>.Builder()
            .SortBy("Title")
            .ThenBy("Age")
            .Build();

        var sorted = (List<Person>)list.SortLength(config);

        sorted.ShouldBe([obj4, obj2, obj, obj3]);
    }

    [Fact]
    public void Should_sort_descending_when_configured()
    {
        var obj = new Person { Name = "Alice", Title = "Engineer", Age = 30 };
        var obj2 = new Person { Name = "Bob", Title = "Manager", Age = 25 };
        var obj3 = new Person { Name = "Charlie", Title = "Engineer", Age = 35 };
        var obj4 = new Person { Name = "Alice", Title = "Analyst", Age = 28 };
        List<Person> list = [obj, obj2, obj3, obj4];

        var config = new SortConfig<Person>.Builder()
            .SortBy(p => p.Name).ThenBy(p => p.Age)
            .SortAscending(false)
            .Build();

        var sorted = list.SortLength(config);

        list.ShouldBe([obj3, obj, obj4, obj2]);
    }

    [Fact]
    public void Should_return_queue_when_configured()
    {
        var obj = new Person { Name = "Alice", Title = "Engineer", Age = 30 };
        var obj2 = new Person { Name = "Bob", Title = "Manager", Age = 25 };
        var obj3 = new Person { Name = "Charlie", Title = "Engineer", Age = 35 };
        var obj4 = new Person { Name = "Alice", Title = "Analyst", Age = 28 };
        List<Person> list = [obj, obj2, obj3, obj4];

        var config = new SortConfig<Person>.Builder()
            .SortBy("Name").ThenBy("Age")
            .ReturnType(ReturnType.Queue)
            .Build();

        var sorted = list.SortLength(config);
        list.ShouldBe([obj2, obj4, obj, obj3]);
        sorted.ShouldBeOfType<Queue<Person>>();
    }

    [Fact]
    public void Should_sort_primitive_values_when_no_path_given()
    {
        var list = new List<string> { "aaa", "z", "bbbb", "c" };

        var config = new SortConfig<string>.Builder().Build();

        var sorted = (List<string>)list.SortLength(config);

        sorted.ShouldBe(["c", "z", "aaa", "bbbb"]);
    }
}