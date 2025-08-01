# Teracura.AbstractSort

<details>
  <summary><h2>OverView</h2></summary>

  <p><strong>Teracura.AbstractSort</strong> is a lightweight and flexible sorting utility for .NET that offers intuitive sorting methods with configurable return types. It extends standard <code>List&lt;T&gt;</code> collections with convenient sorting logic using method chaining and supports multiple output formats such as <code>List</code>, <code>Queue</code>, <code>Stack</code>, and <code>HashSet</code>.</p>

- **GitHub**: [https://github.com/Teracura/Teracura.AbstractSort](https://github.com/Teracura/Teracura.AbstractSort)
- **Documentation**: [https://github.com/Teracura/Teracura.AbstractSort/blob/master/Documentation.md](https://github.com/Teracura/Teracura.AbstractSort/blob/master/Documentation.md)

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
list.SortLength(); // sorts in-place based on length
// list turns to ["fig", "kiwi", "apple","banana"]
```

`SortLength()` sorts the original list in-place, and optionally returns a new collection based on the specified `ReturnType`

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
//HashSet includes [null, 2.3, 3.5, double.NaN, double.E, double.MaxValue, double.MinValue]
```
Note: null is treated as a value of length `-1` and will **not** throw an exception

</details>


