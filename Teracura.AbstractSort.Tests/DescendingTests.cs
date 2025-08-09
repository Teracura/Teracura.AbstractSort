using Shouldly;
using Teracura.AbstractSort.Logic;
using Teracura.AbstractSort.Logic.Configurations;
using Teracura.AbstractSort.Logic.Sorting;

namespace Teracura.AbstractSort.Tests;

public class DescendingTests
{
    [Fact]
    public void Should_Sort_Config_Descending_Queue()
    {
        var config = new SortConfig<int>.Builder().Ascending(false).Mode(SortMode.Length).ReturnType(ReturnType.Queue).Build();
        config.Ascending.ShouldBeFalse();
        config.ReturnType.ShouldBe(ReturnType.Queue);
        List<int> list = [1, 2, 3, 4, 5, 6, 9, 8, 7, 10];
        var obj = list.Sort<int>(config);
        obj.ShouldBeOfType<Queue<int>>();
        list.ShouldBe([10, 9, 8, 7, 6, 5, 4, 3, 2, 1]);
    }

    [Fact]
    public void Should_Sort_Config_Descending_HashSet()
    {
        var config = new SortConfig<int>.Builder().Ascending(false).Mode(SortMode.Length).ReturnType(ReturnType.HashSet).Build();
        config.Ascending.ShouldBeFalse();
        config.ReturnType.ShouldBe(ReturnType.HashSet);
        List<int> list = [1, 2, 3, 4, 5, 6, 9, 8, 7, 10];
        var obj = list.Sort(config);
        obj.ShouldBeOfType<HashSet<int>>();
        list.ShouldBe([10, 9, 8, 7, 6, 5, 4, 3, 2, 1]);
    }

    [Fact]
    public void Should_Sort_Config_Descending_Stack()
    {
        var config = new SortConfig<int>.Builder().Ascending(false).Mode(SortMode.Length).ReturnType(ReturnType.Stack).Build();
        config.Ascending.ShouldBeFalse();
        config.ReturnType.ShouldBe(ReturnType.Stack);
        List<int> list = [1, 2, 3, 4, 5, 6, 9, 8, 7, 10];
        var obj = list.Sort(config);
        obj.ShouldBeOfType<Stack<int>>();
        list.ShouldBe([10, 9, 8, 7, 6, 5, 4, 3, 2, 1]);
    }
}