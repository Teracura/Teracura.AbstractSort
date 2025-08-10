using Teracura.AbstractSort.Logic.Configurations;

namespace Teracura.AbstractSort.Logic.Sorting;

internal static class SortingUtils
{
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

    private static IOrderedEnumerable<T> OrderByLengthThenAlpha<T>( //alpha standing for alphanumerical
        IEnumerable<T> list,
        Func<T, object?> selector, bool caseSensitive)
    {
        return list
            .OrderBy(x => selector(x)?.ToString()?.Length ?? -1)
            .ThenBy(x => selector(x)?.ToString() ?? "",
                caseSensitive ? StringComparer.Ordinal : StringComparer.OrdinalIgnoreCase);
    }

    private static IOrderedEnumerable<T> ThenByLengthThenAlpha<T>(IOrderedEnumerable<T> ordered,
        Func<T, object?> selector, bool caseSensitive)
    {
        return ordered
            .ThenBy(x => selector(x)?.ToString()?.Length ?? -1)
            .ThenBy(x => selector(x)?.ToString() ?? "",
                caseSensitive ? StringComparer.Ordinal : StringComparer.OrdinalIgnoreCase);
    }

    internal static List<T> SortByLength<T>(List<T> list, SortConfig<T> config)
    {
        var caseSensitive = config.CaseSensitive;
        var selectors = config.SortingMethod switch
        {
            SortingMethods.Lambda => config.LambdaSelectors,
            SortingMethods.Reflection => BuildReflectionSelectors(list, config),
            _ => throw new ArgumentOutOfRangeException()
        };

        if (selectors.Count == 0 || selectors[0] == null)
            throw new ArgumentException("No selectors provided to sort based on");

        var ordered = OrderByLengthThenAlpha(list, selectors[0]!, caseSensitive);

        for (var i = 1; i < selectors.Count; i++)
        {
            var sel = selectors[i];
            if (sel != null)
                ordered = ThenByLengthThenAlpha(ordered, sel, caseSensitive);
        }

        return ordered.ToList();
    }

    private static List<Func<T, object?>?> BuildReflectionSelectors<T>(List<T> list, SortConfig<T> config)
    {
        if (config.ReflectionPaths.Count != 0)
        {
            return config.ReflectionPaths
                .Select(path => (Func<T, object?>)(x => GetPropertyValue(x, path)))
                .Cast<Func<T, object?>?>()
                .ToList();
        }

        // No property paths provided — allow sorting only for primitive types
        CheckForPrimitiveValue(typeof(T));
        return [x => x];
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

    internal static object ReturnFromType<T>(ReturnType returnType, List<T> sorted)
    {
        return returnType switch
        {
            ReturnType.List => sorted,
            ReturnType.Queue => new Queue<T>(sorted),
            ReturnType.Stack => new Stack<T>(sorted),
            ReturnType.HashSet => new HashSet<T>(sorted),
            _ => throw new ArgumentOutOfRangeException(nameof(returnType),
                $"Unknown return type: {returnType}")
        };
    }

    internal static int GetPartValue(string version, int index)
    {
        var parts = version.Split('.');
        if (index < parts.Length && int.TryParse(parts[index], out var value))
            return value;
        return 0; // Treat missing parts as 0
    }
}