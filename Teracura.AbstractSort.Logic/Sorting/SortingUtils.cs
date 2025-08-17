using System.Reflection;
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
            SortingMethods.Reflection => BuildReflectionSelectors(config),
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

    internal static List<Func<T, object?>?> BuildReflectionSelectors<T>(SortConfig<T> config)
    {
        if (config.ReflectionPaths.Count != 0)
        {
            return config.ReflectionPaths
                .Select(path => (Func<T, object?>)(x => GetPropertyValue(x, path, config.AllowPrivateAccess)))
                .Cast<Func<T, object?>?>()
                .ToList();
        }

        // No property paths provided — allow sorting only for primitive types
        CheckForPrimitiveValue(typeof(T));
        return [x => x];
    }

    private static object? GetPropertyValue(object? obj, string propertyPath, bool allowPrivateAccess)
    {
        if (obj == null) return null;

        var parts = propertyPath.Split('.');
        var current = obj;
        
        var bindingFlags = BindingFlags.Instance | BindingFlags.Public;
        if (allowPrivateAccess)
        {
            bindingFlags |= BindingFlags.NonPublic;
        }

        foreach (var part in parts)
        {
            if (current == null) return null;
            var type = current.GetType();
            var prop = type.GetProperty(part, bindingFlags);
            if (prop != null)
            {
                current = prop.GetValue(current);
                continue;
            }

            var field = type.GetField(part, bindingFlags);

            if (field != null)
            {
                current = field.GetValue(current);
                continue;
            }

            var defaultMembers = type.GetDefaultMembers();
            var indexer = defaultMembers
                .OfType<PropertyInfo>()
                .FirstOrDefault(p => p.GetIndexParameters().Length == 1);

            if (indexer != null)
            {
                var indexParams = indexer.GetIndexParameters();
                var indexType = indexParams[0].ParameterType;

                object? indexValue = null;
                if (indexType == typeof(int) && int.TryParse(part, out var i))
                    indexValue = i;
                else if (indexType == typeof(string))
                    indexValue = part;

                if (indexValue != null)
                {
                    current = indexer.GetValue(current, [indexValue]);
                    continue;
                }
            }

            throw new ArgumentException($"Member '{part}' not found on type '{type.Name}'");
        }

        return current;
    }

    internal static IEnumerable<T> ReturnFromType<T>(ReturnType returnType, IEnumerable<T> sorted)
    {
        return returnType switch
        {
            ReturnType.List => sorted.ToList(),
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

    internal static void MutateOriginal<T>(IEnumerable<T> enumerable, List<T> items)
    {
        switch (enumerable)
        {
            case ICollection<T> coll and not T[]:
            {
                coll.Clear();
                foreach (var item in items)
                    coll.Add(item);
                break;
            }
            case T[] arr:
            {
                var sortedArr = items.ToArray();
                Array.Copy(sortedArr, arr, arr.Length);
                break;
            }
            default:
            {
                System.Diagnostics.Debug.WriteLine(
                    $"[AbstractSorter] Warning: Cannot mutate type {enumerable.GetType().Name}, returning new sorted collection instead."
                );
                break;
            }
        }
    }
}