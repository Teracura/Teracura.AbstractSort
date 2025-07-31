using Shouldly;

namespace Teracura.AbstractSort.Tests;

public class AscendingTests
{
    [Fact]
    public void Should_Sort_Ascending_Lexicographically()
    {
        List<string> list = ["Apple", "Banana", "EggPlant", "BombasticSideEye"];
        list.Sort();
        list.ShouldBe(["Apple", "Banana", "BombasticSideEye", "EggPlant"]);
    }

    [Fact]
    public void Should_Sort_Ascending_Length_Strings()
    {
        List<string> list = ["Apple", "EggPlant", "Banana", "BombasticSideEye"];
        list = SortingMethods.SortLength(list);
        list.ShouldBe(["Apple", "Banana", "EggPlant", "BombasticSideEye"]);
        //making unique names for each will be hell
    }

    [Fact]
    public void Should_Sort_Ascending_Length_Strings_With_Nulls()
    {
        List<string?> list = [null, "", " ", "a", "ba"];
        list = SortingMethods.SortLength(list);
        list.ShouldBe([null, "", " ", "a", "ba"]);
    }

    [Fact]
    public void Should_Sort_Ascending_Length_Ints()
    {
        List<int> list = [1, 1, 2, 10, 9032, 0, -13];
        list = SortingMethods.SortLength(list);
        list.ShouldBe([0, 1, 1, 2, 10, -13, 9032]);
    }

    [Fact]
    public void Should_Sort_Ascending_Length_Doubles_With_Weird_Values()
    {
        List<double?> list = [null, double.E, double.MaxValue, double.MinValue, double.NaN, 3.5, 2.3, 00002.3];
        list = SortingMethods.SortLength(list);
        list.ShouldBe([null, 2.3, 00002.3, 3.5, double.NaN, double.E, double.MaxValue, double.MinValue]);
    }
}

public static class SortingMethods
{
    public static List<T> SortLength<T>(List<T> list)
    {
        return list.OrderBy(item => item?.ToString()?.Length ?? -1)
            .ThenBy(item => item?.ToString())
            .ToList();
    }
}