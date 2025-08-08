namespace Teracura.AbstractSort.Logic;

public static class AbstractSorter
{
    public static object SortLength<T>(this List<T> list, SortConfig<T>? config = null)
    {
        config ??= new SortConfig<T>.Builder().Build();

        var usePath = config.UseReflectionPath;
        var useLambda = config.UsePropertyExpression;
        var ascending = config.Ascending;
        var returnType = config.ReturnType;

        List<T> sorted = null!;

        if (usePath)
        {
            sorted = SortByLengthReflection(list, config.ReflectionPaths);
        }
        else if (useLambda)
        {
            sorted = SortByLengthLambda(list, config.LambdaSelectors);
        }

        if (!ascending)
            sorted.Reverse();

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

    private static List<T> SortByLengthReflection<T>(List<T> list, List<string> propertyPaths)
    {
        if (propertyPaths.Count == 0)
        {
            CheckForPrimitiveValue(typeof(T));
            return list.OrderBy(x => x?.ToString()?.Length ?? -1)
                .ThenBy(x => x?.ToString())
                .ToList();
        }

        var ordered = list.OrderBy(x =>
        {
            var value = GetPropertyValue(x, propertyPaths[0]);
            return value?.ToString()?.Length ?? -1;
        }).ThenBy(x =>
        {
            var value = GetPropertyValue(x, propertyPaths[0]);
            return value?.ToString() ?? "";
        });

        for (var i = 1; i < propertyPaths.Count; i++)
        {
            var path = propertyPaths[i];
            ordered = ordered.ThenBy(x =>
            {
                var value = GetPropertyValue(x, path);
                return value?.ToString()?.Length ?? -1;
            }).ThenBy(x =>
            {
                var value = GetPropertyValue(x, path);
                return value?.ToString() ?? "";
            });
        }

        return ordered.ToList();
    }

    private static List<T> SortByLengthLambda<T>(List<T> list, List<Func<T, object?>?> selectors)
    {
        if (selectors.Count == 0 || selectors[0] == null)
            throw new ArgumentException("At least one lambda selector must be provided");

        var first = selectors[0]!;
        var ordered = list.OrderBy(x =>
        {
            var value = first(x);
            return value?.ToString()?.Length ?? -1;
        }).ThenBy(x =>
        {
            var value = first(x);
            return value?.ToString() ?? "";
        });

        for (var i = 1; i < selectors.Count; i++)
        {
            var sel = selectors[i];
            if (sel == null) continue;

            ordered = ordered.ThenBy(x =>
            {
                var value = sel(x);
                return value?.ToString()?.Length ?? -1;
            }).ThenBy(x =>
            {
                var value = sel(x);
                return value?.ToString() ?? "";
            });
        }

        return ordered.ToList();
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