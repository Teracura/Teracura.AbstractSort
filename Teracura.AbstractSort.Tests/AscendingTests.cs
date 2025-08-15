using Shouldly;
using Teracura.AbstractSort.Logic;
using Teracura.AbstractSort.Logic.Configurations;
using Teracura.AbstractSort.Logic.Sorting;

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
        var config = new SortConfig<string>.Builder().MutateOriginal().Mode(SortMode.Length).Build();
        list.Sort(config);
        list.ShouldBe(["Apple", "Banana", "EggPlant", "BombasticSideEye"]);
    }

    [Fact]
    public void Should_Sort_Ascending_Length_Strings_With_Nulls()
    {
        List<string?> list = [null, "", " ", "a", "ba"];
        var config = new SortConfig<string>.Builder().Mode(SortMode.Length).MutateOriginal().Build();
        list!.Sort(config); //null handled already
        list.ShouldBe([null, "", " ", "a", "ba"]);
    }

    [Fact]
    public void Should_Sort_Ascending_Length_Ints()
    {
        List<int> list = [1, 1, 2, 10, 9032, 0, -13];
        var config = new SortConfig<int>.Builder().Mode(SortMode.Length).MutateOriginal().Build();
        list.Sort(config);
        list.ShouldBe([0, 1, 1, 2, 10, -13, 9032]);
    }

    [Fact]
    public void Should_Sort_Ascending_Length_Doubles_With_Weird_Values()
    {
        List<double?> list = [null, double.E, double.MaxValue, double.MinValue, double.NaN, 3.5, 2.3, 00002.3];
        var config = new SortConfig<double?>.Builder().Mode(SortMode.Length).MutateOriginal().Build();
        list.Sort(config);
        list.ShouldBe([null, 2.3, 00002.3, 3.5, double.NaN, double.E, double.MaxValue, double.MinValue]);
    }

    [Fact]
    public void Should_Sort_Ascending_Length_Objects()
    {
        var obj = new TestClass("Apple", 5, new TestClass2(1));
        var obj2 = new TestClass("EggPlant", 3, new TestClass2(3));
        var obj3 = new TestClass("Banana", -1, new TestClass2(5));
        var obj4 = new TestClass("BombasticSideEye", 10, new TestClass2(4));
        List<TestClass> list = [obj, obj2, obj3, obj4];
        var config = new SortConfig<TestClass>.Builder().SortBy("Name").MutateOriginal().Mode(SortMode.Length).Build();
        var config2 = new SortConfig<TestClass>.Builder().SortBy("Age").MutateOriginal().Mode(SortMode.Length).Build();
        var config3 = new SortConfig<TestClass>.Builder().SortBy("TestClass2.Number").MutateOriginal()
            .Mode(SortMode.Length).Build();
        list.Sort(config);
        list.ShouldBe([obj, obj3, obj2, obj4]);
        list.Sort(config2);
        list.ShouldBe([obj2, obj, obj3, obj4]);
        list.Sort(config3);
        list.ShouldBe([obj, obj2, obj4, obj3]);
    }

    [Fact]
    public void Should_Throw_Exception_On_Invalid_Property_Path()
    {
        var obj = new TestClass("Apple", 5, new TestClass2(1));
        var obj2 = new TestClass("EggPlant", 3, new TestClass2(3));
        var obj3 = new TestClass("Banana", -1, new TestClass2(5));
        var obj4 = new TestClass("BombasticSideEye", 10, new TestClass2(4));
        List<TestClass> list = [obj, obj2, obj3, obj4];
        var config = new SortConfig<TestClass>.Builder().MutateOriginal().SortBy("Name.Name").Mode(SortMode.Length)
            .Build();
        Should.Throw<ArgumentException>(() => list.Sort(config));
    }

    [Fact]
    public void Should_Throw_Exception_On_Invalid_Property_Path_2()
    {
        var obj = new TestClass("Apple", 5, new TestClass2(1));
        var obj2 = new TestClass("EggPlant", 3, new TestClass2(3));
        var obj3 = new TestClass("Banana", -1, new TestClass2(5));
        var obj4 = new TestClass("BombasticSideEye", 10, new TestClass2(4));
        List<TestClass> list = [obj, obj2, obj3, obj4];
        var config = new SortConfig<TestClass>.Builder().MutateOriginal().SortBy("Name.Name.Name").Mode(SortMode.Length)
            .Build();
        Should.Throw<ArgumentException>(() => list.Sort(config));
    }

    [Fact]
    public void Should_Throw_Exception_On_Non_Primitive_Default_Sort()
    {
        var obj = new TestClass("Apple", 5, new TestClass2(1));
        var obj2 = new TestClass("EggPlant", 3, new TestClass2(3));
        var obj3 = new TestClass("Banana", -1, new TestClass2(5));
        var obj4 = new TestClass("BombasticSideEye", 10, new TestClass2(4));
        List<TestClass> list = [obj, obj2, obj3, obj4];
        var config = new SortConfig<TestClass>.Builder().MutateOriginal().Mode(SortMode.Length).Build();
        Should.Throw<InvalidOperationException>(() => list.Sort(config));
    }

    [Fact]
    public void Should_Sort_Primitive_Lambda_Expression()
    {
        List<int> list = [1, 1, 2, 10, 9032, 0, -13];
        var config = new SortConfig<int>.Builder().MutateOriginal().Mode(SortMode.Length).SortBy(x => x).Build();
        list.Sort(config);
        list.ShouldBe([0, 1, 1, 2, 10, -13, 9032]);
        List<double?> list2 = [null, double.E, double.MaxValue, double.MinValue, double.NaN, 3.5, 2.3, 00002.3];
        var config2 = new SortConfig<double?>.Builder().MutateOriginal().Mode(SortMode.Length).SortBy(x => x).Build();
        list2.Sort(config2);
        list2.ShouldBe([null, 2.3, 00002.3, 3.5, double.NaN, double.E, double.MaxValue, double.MinValue]);
    }

    [Fact]
    public void Should_Sort_Primitive_Lambda_Expression_With_Path()
    {
        List<int> list = [1, 1, 2, 10, 9032, 0, -13];
        var config = new SortConfig<int>.Builder().MutateOriginal().SortBy("Age").Mode(SortMode.Length).SortBy(x => x)
            .Build();
        list.Sort(config);
        list.ShouldBe([0, 1, 1, 2, 10, -13, 9032]);
    }

    [Fact]
    public void Should_Sort_Class_Based_On_Lambda_Expression_Name()
    {
        var obj = new TestClass("Apple", 5, new TestClass2(1));
        var obj2 = new TestClass("EggPlant", 3, new TestClass2(3));
        var obj3 = new TestClass("Banana", -1, new TestClass2(5));
        var obj4 = new TestClass("BombasticSideEye", 10, new TestClass2(4));
        List<TestClass> list = [obj, obj2, obj3, obj4];
        var config = new SortConfig<TestClass>.Builder().MutateOriginal().Mode(SortMode.Length).SortBy(x => x.Name)
            .Build();
        list.Sort(config);
        list.ShouldBe([obj, obj3, obj2, obj4]);
    }

    [Fact]
    public void Should_Sort_Class_Based_On_Lambda_Expression_Age()
    {
        var obj = new TestClass("Apple", 5, new TestClass2(1));
        var obj2 = new TestClass("EggPlant", 3, new TestClass2(3));
        var obj3 = new TestClass("Banana", -1, new TestClass2(5));
        var obj4 = new TestClass("BombasticSideEye", 10, new TestClass2(4));
        List<TestClass> list = [obj, obj2, obj3, obj4];
        var config = new SortConfig<TestClass>.Builder().MutateOriginal().Mode(SortMode.Length).SortBy(x => x.Age)
            .Build();
        list.Sort(config);
        list.ShouldBe([obj2, obj, obj3, obj4]);
    }

    [Fact]
    public void Should_Sort_Class_Based_On_Lambda_Expression_TestClass2_Number()
    {
        var obj = new TestClass("Apple", 5, new TestClass2(1));
        var obj2 = new TestClass("EggPlant", 3, new TestClass2(3));
        var obj3 = new TestClass("Banana", -1, new TestClass2(5));
        var obj4 = new TestClass("BombasticSideEye", 10, new TestClass2(4));
        List<TestClass> list = [obj, obj2, obj3, obj4];
        var config = new SortConfig<TestClass>.Builder().MutateOriginal().SortBy(x => x.TestClass2.Number)
            .Mode(SortMode.Length).Build();
        config.SortingMethod.ShouldBe(SortingMethods.Lambda);
        list.Sort(config);
        list.ShouldBe([obj, obj2, obj4, obj3]);
    }

    [Fact]
    public void Should_Sort_Class_Based_On_Property_Path_TestClass2_Number_With_Lambda_Expression()
    {
        var obj = new TestClass("Apple", 5, new TestClass2(1));
        var obj2 = new TestClass("EggPlant", 3, new TestClass2(3));
        var obj3 = new TestClass("Banana", -1, new TestClass2(5));
        var obj4 = new TestClass("BombasticSideEye", 10, new TestClass2(4));
        List<TestClass?> list = [obj, obj2, obj3, obj4];
        var config = new SortConfig<TestClass?>.Builder().MutateOriginal().SortBy(x => x).Mode(SortMode.Length)
            .SortBy("TestClass2.Number")
            .Build();
        config.SortingMethod.ShouldBe(SortingMethods.Reflection);
        list.Sort(config);
        list.ShouldBe([obj, obj2, obj4, obj3]);
    }

    public class TestClass(string? name, int age, TestClass2 testClass2)
    {
        public string? Name { get; set; } = name;
        public int Age { get; set; } = age;
        public TestClass2 TestClass2 { get; set; } = testClass2;
    }

    public class TestClass2(int number)
    {
        public int Number { get; set; } = number;
    }
}