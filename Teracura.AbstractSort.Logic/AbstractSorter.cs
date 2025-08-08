namespace Teracura.AbstractSort.Logic;

public static class AbstractSorter
{
    public static object SortLength<T>(this List<T> list, ReturnType returnType, string? propertyPath)
    {
        var sorted = SortByLength(list, propertyPath);
        list.Clear();
        list.AddRange(sorted);
        return ReturnFromType(returnType, sorted);
    }

    public static object SortLength<T>(this List<T> list, string? propertyPath)
    {
        var sorted = SortByLength(list, propertyPath);
        list.Clear();
        list.AddRange(sorted);
        return ReturnFromType(ReturnType.List, sorted);
    }

    public static object SortLength<T>(this List<T> list, ReturnType returnType)
    {
        var sorted = SortByLength(list, null);
        list.Clear();
        list.AddRange(sorted);
        return ReturnFromType(returnType, sorted);
    }

    public static object SortLength<T>(this List<T> list)
    {
        var sorted = SortByLength(list, null);
        list.Clear();
        list.AddRange(sorted);
        return ReturnFromType(ReturnType.List, sorted);
    }

    private static List<T> SortByLength<T>(List<T> list, string? propertyPath)
    {
        var sorted = string.IsNullOrEmpty(propertyPath)
            ? list.OrderBy(item => item?.ToString()?.Length ?? -1)
                .ThenBy(item => item?.ToString())
                .ToList()
            : list.OrderBy(item =>
                    GetPropertyValue(item, propertyPath)?.ToString()?.Length ?? -1)
                .ThenBy(item =>
                    GetPropertyValue(item, propertyPath)?.ToString() ?? "")
                .ToList();
        return sorted;
    }

    private static object ReturnFromType<T>(ReturnType returnType, List<T> sorted)
    {
        return returnType switch
        {
            ReturnType.List => sorted,
            ReturnType.Queue => new Queue<T>(sorted),
            ReturnType.Stack => new Stack<T>(sorted),
            ReturnType.HashSet => new HashSet<T>(sorted),
            _ => throw new ArgumentOutOfRangeException(nameof(returnType), $"Unknown return type: {returnType}")
        };
    }

    private static object? GetPropertyValue(object? obj, string propertyPath)
    {
        if (obj == null) return null;

        var parts = propertyPath.Split('.');
        var current = obj;

        foreach (var part in parts)
        {
            if (current == null) return null;

            var prop = current.GetType().GetProperty(part);
            if (prop == null) return null;

            current = prop.GetValue(current);
        }

        return current;
    }
}