using Shouldly;
using Teracura.AbstractSort.Logic;

namespace Teracura.AbstractSort.Tests;

public class ReturnTypeTests
{
    [Fact]
    public void Should_Return_Default_List()
    {
        List<string?> list = [null, "", " ", "a", "ba"];
        var obj = list.SortLength();
        list.ShouldBe([null, "", " ", "a", "ba"]);

        obj.ShouldBeOfType<List<string?>>();
    }

    [Fact]
    public void Should_Return_List()
    {
        const ReturnType type = ReturnType.List;
        List<string?> list = [null, "", " ", "a", "ba"];
        var config = new SortConfig.Builder().Build();
        var obj = list.SortLength(config);
        list.ShouldBe([null, "", " ", "a", "ba"]);

        obj.ShouldBeOfType<List<string?>>();
    }

    [Fact]
    public void Should_Return_Queue()
    {
        const ReturnType type = ReturnType.Queue;
        List<int> list = [1, 1, 2, 10, 9032, 0, -13];
        var config = new SortConfig.Builder().ReturnType(type).Build();
        var obj = list.SortLength(config);
        list.ShouldBe([0, 1, 1, 2, 10, -13, 9032]);
        obj.ShouldBeOfType<Queue<int>>();
    }

    [Fact]
    public void Should_Return_Stack()
    {
        const ReturnType type = ReturnType.Stack;
        List<string> list = ["aaa", "b", "cccc"];
        var config = new SortConfig.Builder().ReturnType(type).Build();
        var obj = list.SortLength(config);
        obj.ShouldBeOfType<Stack<string>>();
        list.ShouldBe(["b", "aaa", "cccc"]);
    }

    [Fact]
    public void Should_Return_HashSet()
    {
        const ReturnType type = ReturnType.HashSet;
        List<string> list = ["z", "aaa", "bb", "bb", "z"];
        var config = new SortConfig.Builder().ReturnType(type).Build();
        var obj = list.SortLength(config);
        obj.ShouldBeOfType<HashSet<string>>();
        list.ShouldBe(["z", "z", "bb", "bb", "aaa"]);
        var set = (HashSet<string>)obj!;
        set.Count.ShouldBe(3);
    }
}