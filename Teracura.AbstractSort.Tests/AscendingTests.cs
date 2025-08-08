using Shouldly;
using Teracura.AbstractSort.Logic;

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
        list.SortLength();
        list.ShouldBe(["Apple", "Banana", "EggPlant", "BombasticSideEye"]);
        //making unique names for each will be hell
    }

    [Fact]
    public void Should_Sort_Ascending_Length_Strings_With_Nulls()
    {
        List<string?> list = [null, "", " ", "a", "ba"];
        list.SortLength();
        list.ShouldBe([null, "", " ", "a", "ba"]);
    }

    [Fact]
    public void Should_Sort_Ascending_Length_Ints()
    {
        List<int> list = [1, 1, 2, 10, 9032, 0, -13];
        list.SortLength();
        list.ShouldBe([0, 1, 1, 2, 10, -13, 9032]);
    }

    [Fact]
    public void Should_Sort_Ascending_Length_Doubles_With_Weird_Values()
    {
        List<double?> list = [null, double.E, double.MaxValue, double.MinValue, double.NaN, 3.5, 2.3, 00002.3];
        list.SortLength();
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
        var config = new SortConfig.Builder().SetPropertyPath("Name").Build();
        var config2 = new SortConfig.Builder().SetPropertyPath("Age").Build();
        var config3 = new SortConfig.Builder().SetPropertyPath("TestClass2.Number").Build();
        list.SortLength(config);
        list.ShouldBe([obj, obj3, obj2, obj4]);
        list.SortLength(config2);
        list.ShouldBe([obj2, obj, obj3, obj4]);
        list.SortLength(config3);
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
        var config = new SortConfig.Builder().SetPropertyPath("Name.Name").Build();
        Should.Throw<ArgumentException>(() => list.SortLength(config));
    }

    [Fact]
    public void Should_Throw_Exception_On_Invalid_Property_Path_2()
    {
        var obj = new TestClass("Apple", 5, new TestClass2(1));
        var obj2 = new TestClass("EggPlant", 3, new TestClass2(3));
        var obj3 = new TestClass("Banana", -1, new TestClass2(5));
        var obj4 = new TestClass("BombasticSideEye", 10, new TestClass2(4));
        List<TestClass> list = [obj, obj2, obj3, obj4];
        var config = new SortConfig.Builder().SetPropertyPath("Name.Name.Name").Build();
        Should.Throw<ArgumentException>(() => list.SortLength(config));
    }

    [Fact]
    public void Should_Throw_Exception_On_Non_Primitive_Default_Sort()
    {
        var obj = new TestClass("Apple", 5, new TestClass2(1));
        var obj2 = new TestClass("EggPlant", 3, new TestClass2(3));
        var obj3 = new TestClass("Banana", -1, new TestClass2(5));
        var obj4 = new TestClass("BombasticSideEye", 10, new TestClass2(4));
        List<TestClass> list = [obj, obj2, obj3, obj4];
        Should.Throw<InvalidOperationException>(() => list.SortLength());
    }
    public class TestClass(string name, int age, TestClass2 testClass2)
    {
        public string Name { get; set; } = name;
        public int Age { get; set; } = age;
        public TestClass2 TestClass2 { get; set; } = testClass2;
    }

    public class TestClass2(int number)
    {
        public int Number { get; set; } = number;
    }
}