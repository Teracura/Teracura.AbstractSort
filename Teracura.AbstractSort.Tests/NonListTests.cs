using Teracura.AbstractSort.Logic.Configurations;
using Teracura.AbstractSort.Logic.Sorting;

namespace Teracura.AbstractSort.Tests;

using Shouldly;

public class NonListTests
{
    [Fact]
    public void Sorts_Array_DefaultComparer()
    {
        var arr = new[] { 5, 2, 9, 1 };
        var config = new SortConfig<int>.Builder()
            .SortBy(x => x)
            .Build();

        var result = arr.Sort(config).ToArray();

        result.ShouldBe([1, 2, 5, 9]);
    }

    [Fact]
    public void Sorts_Queue_Ascending()
    {
        var queue = new Queue<string>(["delta", "alpha", "charlie"]);
        var config = new SortConfig<string>.Builder()
            .SortBy(x => x)
            .Ascending(true)
            .Build();

        var result = queue.Sort(config).ToList();

        result.ShouldBe(new[] { "alpha", "charlie", "delta" });
    }

    [Fact]
    public void Sorts_Stack_Descending()
    {
        var stack = new Stack<int>([3, 1, 2]);
        var config = new SortConfig<int>.Builder()
            .SortBy(x => x)
            .Ascending(false)
            .Build();

        var result = stack.Sort(config).ToList();

        result.ShouldBe([3, 2, 1]
        );
    }

    [Fact]
    public void Sorts_LinkedList_ByStringLength()
    {
        var linked = new LinkedList<string>(["bbb", "a", "cc"]);
        var config = new SortConfig<string>.Builder()
            .SortBy(x => x.Length)
            .Build();

        var result = linked.Sort(config).ToList();

        result.ShouldBe(["a", "cc", "bbb"]);
    }

    [Fact]
    public void Sorts_HashSet_ByLength()
    {
        var set = new HashSet<string> { "xxxx", "yy", "zzz" };
        var config = new SortConfig<string>.Builder()
            .SortBy(x => x.Length)
            .Build();

        var result = set.Sort(config).ToList();

        result.ShouldBe(["yy", "zzz", "xxxx"]);
    }

    [Fact]
    public void Sorts_Array_MutateOriginal_Works()
    {
        var arr = new[] { 3, 1, 2 };
        var config = new SortConfig<int>.Builder()
            .SortBy(x => x)
            .MutateOriginal(true)
            .Build();

        var result = arr.Sort(config);

        arr.ShouldBe([1, 2, 3]);
        result.ShouldBe([1, 2, 3]);
    }

    [Fact]
    public void Sorts_Queue_MutateOriginal_ShouldWarnAndNotMutate()
    {
        var queue = new Queue<int>([3, 1, 2]);
        var config = new SortConfig<int>.Builder()
            .SortBy(x => x)
            .MutateOriginal(true)
            .Build();

        var result = queue.Sort(config).ToList();

        // Sorted result is correct
        result.ShouldBe([1, 2, 3]);

        // But original queue is NOT sorted, since Queue<T> cannot be mutated
        queue.ToArray().ShouldBe([3, 1, 2]);
    }
}