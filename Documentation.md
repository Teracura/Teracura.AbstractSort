# Teracura.AbstractSort

<details>
  <summary><h2>Overview</h2></summary>

  <p><strong>Teracura.AbstractSort</strong> is a lightweight and flexible sorting utility for .NET that offers intuitive sorting methods with configurable return types. It extends standard <code>List&lt;T&gt;</code> collections with convenient sorting logic using method chaining and supports multiple output formats such as <code>List</code>, <code>Queue</code>, <code>Stack</code>, and <code>HashSet</code>.</p>

- **GitHub**: [https://github.com/Teracura/Teracura.AbstractSort](https://github.com/Teracura/Teracura.AbstractSort)
- **Documentation
  **: [https://github.com/Teracura/Teracura.AbstractSort/blob/master/Documentation.md](https://github.com/Teracura/Teracura.AbstractSort/blob/master/Documentation.md)

</details>


<details>
<summary><h2>Details</h2></summary>

- Extension-based syntax: `list.SortLength()`
- Automatic sorting by string length then alphabetical order
- Preserves original list ordering by reference
- Supports custom return types
- Immutable-friendly internals using `System.Collections.Immutable`

</details>

<details>
<summary><h2>Usage</h2></summary>

<h3>SortLength</h3>
<strong>Sorts in-place based on length then alphabetical order</strong>

```csharp
using Teracura.AbstractSort.Logic;

List<string> list = ["banana", "kiwi", "apple", "fig"];
list.SortLength(); // sorts in-place by string length, then lexicographically
// list turns to ["fig", "kiwi", "apple","banana"]
```

`SortLength()` sorts the original list in-place, and optionally returns a new collection based on the specified
`ReturnType`

<h3>ReturnType</h3>
<strong>Enum with multiple return types</strong>
Current return types:

- `List`
- `Queue`
- `Stack`
- `HashSet`

can be used as a parameter to `List.SortLength(ReturnType)`

```csharp
List<double> list = [null, double.E, double.MaxValue, double.MinValue, double.NaN, 3.5, 2.3, 2.3, 00002.3];
ReturnType type = ReturnType.HashSet;
HashSet<double> set = list.SortLength(type); //list turns to [null, 2.3, 2.3, 2.3, 3.5, double.NaN, double.E, double.MaxValue, double.MinValue]
// HashSet contains distinct elements in sorted order by string length (null is included with length -1)
```

Note: null is treated as a value of `length: -1` and will **not** throw an exception

Note: `SortLength(input parameters)` sorts based on the **string length** of the property value **then** numeric or natural order. Which means that `-1` is considered longer than `1` in numerical values, while `10` is sorted bigger than `-1` because both are `length: 2` but `10` is numerically larger than `-1`

<h3>Custom Sorting</h3>
<strong>Can use reflection to sort based on any primitive value in a class, including a nested classes</strong>

```csharp
        //TestClass parameters are (string: Name,int: Age, TestClass2: TestClass2)
        //TestClass2 parameters are(int: Number)
        var obj = new TestClass("Apple", 5, new TestClass2(1));
        var obj2 = new TestClass("EggPlant", 3, new TestClass2(3));
        var obj3 = new TestClass("Banana", -1, new TestClass2(5));
        var obj4 = new TestClass("BombasticSideEye", 10, new TestClass2(4));
        List<TestClass> list = [obj, obj2, obj3, obj4];
        list.SortLength("Name"); //expected: [obj, obj3, obj2, obj4], sorts by Name parameter
        list.SortLength("Age"); //expected: [obj2, obj, obj3, obj4]
        list.SortLength("TestClass2.Number"); //expected [obj, obj2, obj4, obj3]
```

<h3>In summary</h3>

- Class: `AbstractSorter`
  - Method: `SortLength()`
  - Method: `SortLength(ReturnType: type)`
  - Method: `SortLength(string: propertyPath)`
  - Method: `SortLength(string: propertyPath, ReturnType: type)`
- Enum: `ReturnType`
  - Values: `List`, `Queue`, `Stack`, `HashSet`
</details>


