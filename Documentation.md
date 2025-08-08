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

`list` is sorted in-place, and a separate collection (based on the `ReturnType`) is returned, which does not affect the
original list.

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

Note: `SortLength(input parameters)` sorts based on the **string length** of the property value **then** numeric or
natural order. Which means that `-1` is considered longer than `1` in numerical values, while `10` is sorted bigger than
`-1` because both are `length: 2` but `10` is numerically larger than `-1`

### SortConfig

`SortConfig` is a class that uses builder pattern to be initialized, used for more customized sorting

how to initialize:

```csharp
SortConfig configuration = new SortConfig.Builder().Build(); //for default case
```

**defaults will have the following datatypes:**

- `List<string?>`: ReflectionPaths = `[]`
- `List<Func<T,object?>?>`: LambdaSelectors = `[]`
- `bool`: UseReflectionPath = `true`
- `bool`: UsePropertyExpression = `false`
- `bool`: Ascending = `true`
- `ReturnType`: ReturnType = `ReturnType.List`

**SortConfig subclasses:**

- `Builder` builder pattern used to construct the configuration

**Builder Methods**

- `SortBy(string path)` sets first value of `ReflectionPaths` to the given path and sets `UseReflectionPath` to `true` and
  the rest of the sort types to `false`
- `SortBy(Func<T,object?>? expression)` sets `LambaSelectors` to given lambda expression and sets
  `UsePropertyExpression` to `true` and the rest of the sort types to `false`
- `ThenBy(string path)` adds the given path to `Reflection paths`. Only works when `SortBy(string path)` is invoked first
- `ThenBy(Func<T,object?>?) expression` adds given lambda expression to `LambdaSelectors`. only works when
  `SortBy(Func<T,object?>? expression)` is invoked first
- `SortAscending(bool ascending = true)` sets `Ascending` to given input
- `ReturnType(ReturnType type = Logic.ReturnType.List)` sets `ReturnType` to given input
- `Build()` returns a new `SortConfig` class with the changed datatypes (or default if no datatypes are changed)

<h3>Custom Sorting</h3>
**Can use reflection to sort based on any primitive value in an object, including nested classes**

```csharp
//TestClass parameters are (string: Name,int: Age, TestClass2: TestClass2)
//TestClass2 parameters are(int: Number)
var obj = new TestClass("Apple", 5, new TestClass2(1));
var obj2 = new TestClass("EggPlant", 3, new TestClass2(3));
var obj3 = new TestClass("Banana", -1, new TestClass2(5));
var obj4 = new TestClass("BombasticSideEye", 10, new TestClass2(4));
List<TestClass> list = [obj, obj2, obj3, obj4];
var config = new SortConfig.Builder().SetPropertyPath("Name").Build();
var config2 = new SortConfig.Builder().SetPropertyPath("Age").Build();
var config3 = new SortConfig.Builder().SetPropertyPath("TestClass2.Number").Build();
list.SortLength(config); //expected: [obj, obj3, obj2, obj4], sorts by Name parameter
list.SortLength(config2); //expected: [obj2, obj, obj3, obj4]
list.SortLength(config3); //expected [obj, obj2, obj4, obj3]
```

**Can also use Lambda expressions to sort based on any primitive value in an object, including nested classes**

```csharp
var obj = new TestClass("Apple", 5, new TestClass2(1));
var obj2 = new TestClass("EggPlant", 3, new TestClass2(3));
var obj3 = new TestClass("Banana", -1, new TestClass2(5));
var obj4 = new TestClass("BombasticSideEye", 10, new TestClass2(4));
List<TestClass> list = [obj, obj2, obj3, obj4];
var config = new SortConfig<TestClass>.Builder().SortBy(x => x.Name).Build();
list.SortLength(config); //expected: [obj, obj3, obj2, obj4]
```

**Can also chain Lambda expressions or reflection paths using ThenBy in SortConfig, but you cannot combine both methods
**

```csharp
var obj = new Person { Name = "Alice", Title = "Engineer", Age = 30 };
var obj2 = new Person { Name = "Bob", Title = "Manager", Age = 25 };
var obj3 = new Person { Name = "Charlie", Title = "Engineer", Age = 35 };
var obj4 = new Person { Name = "Alice", Title = "Analyst", Age = 28 };
List<Person> list = [obj, obj2, obj3, obj4];

var config = new SortConfig<Person>.Builder()
    .SortBy("Title")
    .ThenBy("Age")
    .Build();
var sorted = list.SortLength(config); //expected [obj4, obj2, obj, obj3]
```

or in case of lambda

```csharp
var obj = new Person { Name = "Alice", Title = "Engineer", Age = 30 };
var obj2 = new Person { Name = "Bob", Title = "Manager", Age = 25 };
var obj3 = new Person { Name = "Charlie", Title = "Engineer", Age = 35 };
var obj4 = new Person { Name = "Alice", Title = "Analyst", Age = 28 };
List<Person> list = [obj, obj2, obj3, obj4];

var config = new SortConfig<Person>.Builder()
    .SortBy(p => p.Name)
    .ThenBy(p => p.Title)
    .Build();
var sorted = list.SortLength(config); //expected [obj4, obj2, obj, obj3]
```
Notes:
- You may chain as much sorting conditions as you want with ThenBy, you are not limited to one
- You cannot use a lambda expression on SortBy then use string reflections on ThenBy, you will receive an exception, and vise-versa
- While you are allowed to pass an empty string (or none at all) to string reflection, it will sort as usual only for primitive values, while for non-primitive values you will receive an exception (Null is still accepted, as well as string)

<h3>In summary</h3>

- Class: `AbstractSorter`
    - Method: `SortLength(SortConfig: config = default)` returns value based on `ReturnType`
- Enum: `ReturnType`
    - Values: `List`, `Queue`, `Stack`, `HashSet`
- Class: `SortConfig`
    - string: `ReflectionPath`
    - bool: `UsePath`
    - bool: `Ascending`
    - Enum: ReturnType: `ReturnType`
    - Class: Builder:
        - Method: `SortBy(string: path)`
        - Method: `SortBy(Func<T,object?>?: expression)`
        - Method: `ThenBy(string: path)`
        - Method: `ThenBy(Func<T,object?>?: expression)`
        - Method: `SortAscending(bool: ascending = true)`
        - Method: `ReturnType(ReturnType: type = ReturnType.List)`
        - Method: `Build()` returns `SortConfig`

</details>


