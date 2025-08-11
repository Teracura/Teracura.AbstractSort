using Shouldly;
using Teracura.AbstractSort.Logic.Configurations;
using Teracura.AbstractSort.Logic.Sorting;

namespace Teracura.AbstractSort.Tests;

public class DefaultSortTests
{
    [Fact]
    public void Should_Sort_List_With_Nulls_And_Ints()
    {
        var config = new SortConfig<int?>.Builder().MutateOriginal().Build();
        List<int?> list = [9, 0, 8, 5, 4, 2, null];
        list.Sort(config);
        list.ShouldBe([null, 0, 2, 4, 5, 8, 9]);
    }

    [Fact]
    public void Should_Sort_List_With_Nulls_And_Strings()
    {
        var config = new SortConfig<string?>.Builder().MutateOriginal().Build();
        List<string?> list = ["", " ", "a", "b", "c", null];
        list.Sort(config);
        list.ShouldBe([null, "", " ", "a", "b", "c"]);
    }

    [Fact]
    public void Should_Sort_List_With_Nulls_And_DateTimes()
    {
        var config = new SortConfig<DateTime?>.Builder().MutateOriginal().SortBy(x => x).Build();
        List<DateTime?> list =
        [
            new DateTime(2022, 1, 1), null, new DateTime(2022, 1, 2), new DateTime(2022, 1, 3),
            new DateTime(2021, 5, 29)
        ];
        list.Sort(config);
        list.ShouldBe([
            null, new DateTime(2021, 5, 29), new DateTime(2022, 1, 1), new DateTime(2022, 1, 2),
            new DateTime(2022, 1, 3)
        ]);
    }

    [Fact]
    public void Should_Sort_List_With_Cursed_Values()
    {
        var config = new SortConfig<object?>.Builder().MutateOriginal().SortBy(x => x).Build();
        List<object?> list = ["", 2, 5, " ", "eee", null, double.NaN, double.PositiveInfinity, double.NegativeInfinity];
        list.Sort(config);
        list.ShouldBe([null, double.NegativeInfinity, 2, 5, double.PositiveInfinity, double.NaN, "", " ", "eee"]);
    }

    [Fact]
    public void Pain()
    {
        var config = new SortConfig<object?>.Builder().MutateOriginal().SortBy(x => x).Build();
        var crazyList = new List<object?>
        {
            null,
            "",
            " ",
            "𝄞",
            "🐍",
            "normal string",
            0,
            -42,
            123456789012345L,
            3.14159,
            -3.14f,
            decimal.One,
            double.NaN,
            double.PositiveInfinity,
            double.NegativeInfinity,
            true,
            false,
            new DateTime(2000, 1, 1),
            new DateTime(1999, 12, 31),
            (42, "tuple"),
            new int[] { 1, 2, 3 },
            Guid.Empty,
            TimeSpan.Zero,
            new Guid("7bf2a6c1-04af-44a8-954e-3abed16ec432")
        };
        crazyList.Sort(config);
        crazyList.ShouldBe([
            null,
            double.NegativeInfinity,
            -42,
            -3.14f,
            0,
            decimal.One,
            3.14159,
            123456789012345L,
            double.PositiveInfinity,
            double.NaN,
            false,
            true,
            "",
            " ",
            "normal string",
            "𝄞",
            "🐍",
            new DateTime(1999, 12, 31),
            new DateTime(2000, 1, 1),
            Guid.Empty,
            new Guid("7bf2a6c1-04af-44a8-954e-3abed16ec432"),
            TimeSpan.Zero,
            new int[] { 1, 2, 3 },
            (42, "tuple")
        ]);
    }
}