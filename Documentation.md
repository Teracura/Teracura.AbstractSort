# Teracura.AbstractSort

## Overview

**Teracura.AbstractSort** is a lightweight and extensible sorting utility for .NET that provides a fluent,
configuration-driven API for sorting collections.  
It supports:

- Sorting by property paths (**Reflection**) or by **Lambda expressions**.
- Multiple sorting modes:
    - **Default** (type-aware, custom comparer for mixed types)
    - **Length** (string length → alphabetical)
    - **Version** (semantic-like version number ordering)
- Multiple return types: `List`, `Queue`, `Stack`, `HashSet`.
- Optional in-place mutation of the original collection.
- Case sensitivity control.
- Multi-level sorting with `SortBy` + `ThenBy` chaining.
- Robust handling of null values, NaN, and infinities.

---

## Core API

### `AbstractSorter.Sort<T>(this List<T> list, SortConfig<T> config)`

Sorts a `List<T>` according to the provided configuration.

#### Parameters

- **list** — The list to be sorted.
- **config** — A `SortConfig<T>` instance describing sorting mode, selectors, and behavior.

#### Behavior

- Applies sorting based on `SortConfig.SortMode`:
    - `SortMode.None` — Standard ordering via selector(s).
    - `SortMode.Length` — Orders by string length then alphanumerically.
    - `SortMode.Version` — Orders by numeric parts of version strings.
- Sort order is reversed if `Ascending` is `false`.
- If `MutateOriginal` is `true`, modifies the original list in place.
- Returns the sorted data in the format specified by `ReturnType`.

#### Example

```csharp
var people = new List<Person>
{
    new("Alice", 30),
    new("Bob", 25),
    new("Charlie", 35)
};

var config = new SortConfig<Person>.Builder()
    .SortBy(p => p.Name)
    .Ascending(false)
    .ReturnType(ReturnType.Queue)
    .Build();

var queue = (Queue<Person>)people.Sort(config);
```
---

## Sorting Modes

### 1. Default Sorting

* Uses **MultiObjectComparer** for `List<object>` to handle mixed types with a consistent precedence:

    1. null
    2. Numbers (int, long, decimal, float, double)
    3. bool
    4. string
    5. DateTime
    6. Guid
    7. TimeSpan
    8. Array
    9. ValueTuple
    10. Delegate
    11. Other types (by `ToString()`)

```csharp
var items = new List<object?> { null, 42, "apple", 3.14, DateTime.Now };
var config = new SortConfig<object>.Builder().Build();
var sorted = (List<object?>)items.Sort(config);
```

---

### 2. Length Sorting

* Orders by **string length**, then by alphabetical/numeric order.
* Works with selectors from reflection or lambdas.
* Null is treated as length `-1`.

```csharp
var fruits = new List<string> { "banana", "kiwi", "apple", "fig" };
var config = new SortConfig<string>.Builder()
    .Mode(SortMode.Length)
    .Build();

var sorted = (List<string>)fruits.Sort(config);
// Result: ["fig", "kiwi", "apple", "banana"]
```

---

### 3. Version Sorting

* For strings representing dot-separated numbers (`"1.0.3"`, `"1.10"`, `"2.0"`).
* Missing parts are treated as `0`.
* Example: `"1.0"` < `"1.0.1"` < `"1.1"`.

```csharp
var versions = new List<string> { "1.0", "1.0.1", "1.1", "2.0" };
var config = new SortConfig<string>.Builder()
    .Mode(SortMode.Version)
    .Build();

var sorted = (List<string>)versions.Sort(config);
```

---

## SortConfig

### Purpose

`SortConfig<T>` encapsulates all sorting preferences.
It is created via the `SortConfig<T>.Builder` fluent builder.

### Key Properties

| Property          | Default      | Description                                       |
|-------------------|--------------|---------------------------------------------------|
| `ReflectionPaths` | `[]`         | Paths to properties for reflection-based sorting. |
| `LambdaSelectors` | `[]`         | Lambda selectors for sorting.                     |
| `SortingMethod`   | `Reflection` | Reflection or Lambda.                             |
| `SortMode`        | `None`       | Sorting mode (None (default), Length, Version).   |
| `Ascending`       | `true`       | Sort order.                                       |
| `CaseSensitive`   | `true`       | Case sensitivity for string comparison.           |
| `MutateOriginal`  | `false`      | Whether to change original list.                  |
| `ReturnType`      | `List`       | Return type after sorting.                        |

---

### Builder API

| Method                               | Description                                                              |
|--------------------------------------|--------------------------------------------------------------------------|
| `SortBy(string path)`                | Sets primary sort key via reflection path.                               |
| `SortBy(Func<T, object?>? selector)` | Sets primary sort key via lambda.                                        |
| `ThenBy(string path)`                | Adds secondary reflection sort key (requires `SortBy(string)`).          |
| `ThenBy(Func<T, object?>? selector)` | Adds secondary lambda sort key (requires `SortBy(lambda)`).              |
| `Mode(SortMode mode)`                | Sets sorting mode.                                                       |
| `MutateOriginal(bool = true)`        | Enables/disables in-place mutation.                                      |
| `Ascending(bool = true)`             | Sets ascending/descending order. Throws if called >9 times (Easter egg). |
| `CaseSensitive(bool = true)`         | Controls string case sensitivity.                                        |
| `ReturnType(ReturnType type)`        | Sets return type after sorting.                                          |
| `Build()`                            | Returns a configured `SortConfig<T>`.                                    |

---

### Example: Reflection-based Length Sorting

```csharp
var config = new SortConfig<Person>.Builder()
    .SortBy("Name")
    .ThenBy("Age")
    .Mode(SortMode.Length)
    .Build();
```

### Example: Lambda-based Version Sorting

```csharp
var config = new SortConfig<Person>.Builder()
    .SortBy(p => p.VersionString)
    .Mode(SortMode.Version)
    .Build();
```

---

## Return Types

`ReturnType` enum:

* `List`
* `Queue`
* `Stack`
* `HashSet`

Example:

```csharp
var config = new SortConfig<string>.Builder()
    .ReturnType(ReturnType.Stack)
    .Build();
var stack = (Stack<string>)myList.Sort(config);
```

---

## Notes & Rules

* **Cannot mix** reflection and lambda selectors in the same config.
* Chaining with `ThenBy` is unlimited in depth.
* Null values are handled gracefully.
* For `SortMode.Version`, `T` **must** be `string`.
* Reflection selectors can access nested properties (`"Address.City.Name"`).
* When no selector is given for reflection and `T` is not primitive/string, sorting will throw.