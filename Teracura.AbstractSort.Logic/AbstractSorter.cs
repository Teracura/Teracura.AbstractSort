namespace Teracura.AbstractSort.Logic;

public static class AbstractSorter
{
    public static object SortLength<T>(this List<T> list, SortConfig<T>? config = null)
    {
        config ??= new SortConfig<T>.Builder().Build();
        var reflectionPath = config.Path;
        var usePath = config.UseReflectionPath;
        
        // If path is missing but UsePath is enabled, we validate primitive types
        if (string.IsNullOrEmpty(reflectionPath) && usePath)
        {
            var type = typeof(T);
            // Allow only primitive types, string, or nullable of those
            CheckForPrimitiveValue(type);
        }

        var ascending = config.Ascending;
        var returnType = config.ReturnType;
        List<T> sorted = null!; //all null cases will be handled
        if (usePath)
        {
            sorted = SortByLength(list, reflectionPath);
        }

        if (!ascending) sorted.Reverse();

        list.Clear();
        list.AddRange(sorted);

        return ReturnFromType(returnType, sorted);
    }

    private static void CheckForPrimitiveValue(Type type)
    {
        var isValid =
            type.IsPrimitive ||
            type == typeof(string) ||
            (Nullable.GetUnderlyingType(type)?.IsPrimitive ?? false) ||
            Nullable.GetUnderlyingType(type) == typeof(string);

        if (!isValid)
            throw new InvalidOperationException(
                $"Cannot sort objects of type {type.Name} with default SortConfig. " +
                $"Use a property path or lambda expression to define a sort key."
            );
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
            if (prop == null)
                throw new ArgumentException($"Property '{part}' not found on type '{current.GetType().Name}'");

            current = prop.GetValue(current);
        }

        return current;
    }
}