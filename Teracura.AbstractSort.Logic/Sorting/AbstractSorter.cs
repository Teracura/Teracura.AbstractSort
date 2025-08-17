using Teracura.AbstractSort.Logic.Configurations;

namespace Teracura.AbstractSort.Logic.Sorting;

public static class AbstractSorter
{
    public static IEnumerable<T> Sort<T>(this IEnumerable<T> enumerable, SortConfig<T> config)
    {
        var list = enumerable.ToList();
        var sorted = config.SortMode switch
        {
            SortMode.Length => SortLength(list, config),
            SortMode.Version => SortVersion(list, config),
            _ => SortDefault(list, config)
        };

        var items = sorted.ToList();
        if (!config.Ascending)
            items.Reverse();

        if (!config.MutateOriginal) return SortingUtils.ReturnFromType(config.ReturnType, items);

        SortingUtils.MutateOriginal(enumerable, items);

        return SortingUtils.ReturnFromType(config.ReturnType, items);
    }

    private static IEnumerable<T> SortDefault<T>(IEnumerable<T> enumerable, SortConfig<T> config)
    {
        var list = enumerable.ToList();
        if (typeof(T) == typeof(object))
        {
            var comparer = new MultiObjectComparer() as IComparer<T>;
            list.Sort(comparer);
            return list;
        }

        var caseSensitive = config.CaseSensitive;

        var selectors = config.SortingMethod switch
        {
            SortingMethods.Lambda => config.LambdaSelectors,
            SortingMethods.Reflection => SortingUtils.BuildReflectionSelectors(config),
            _ => throw new ArgumentOutOfRangeException()
        };

        ArgumentNullException.ThrowIfNull(list);

        IOrderedEnumerable<T>? ordered = null;

        foreach (var selector in selectors.OfType<Func<T, object?>>())
        {
            var returnsString = list.Select(selector).OfType<string>().Any();

            if (ordered == null)
            {
                if (returnsString)
                {
                    ordered = caseSensitive
                        ? list.OrderBy(x => selector(x) as string)
                        : list.OrderBy(x => (selector(x) as string)?.ToLowerInvariant());
                }
                else
                {
                    ordered = list.OrderBy(x => selector(x));
                }
            }
            else
            {
                if (returnsString)
                {
                    ordered = caseSensitive
                        ? ordered.ThenBy(x => selector(x) as string ?? "")
                        : ordered.ThenBy(x => (selector(x) as string ?? "").ToLowerInvariant());
                }
                else
                {
                    ordered = ordered.ThenBy(x => selector(x));
                }
            }
        }

        return ordered?.ToList() ?? list;
    }


    private static IEnumerable<T> SortVersion<T>(IEnumerable<T> enumerable, SortConfig<T> config)
    {
        if (typeof(T) != typeof(string))
            throw new ArgumentException("Sorting by version only supports strings");
        var list = enumerable.ToList();

        IOrderedEnumerable<string>? ordered = null;

        // Find the maximum number of parts in any version
        var maxParts = list.Cast<string>()
            .Max(v => v.Split('.').Length);

        for (var i = 0; i < maxParts; i++)
        {
            var partIndex = i;
            if (ordered == null)
            {
                ordered = list.Cast<string>()
                    .OrderBy(v => SortingUtils.GetPartValue(v, partIndex));
            }
            else
            {
                ordered = ordered.ThenBy(v => SortingUtils.GetPartValue(v, partIndex));
            }
        }

        return ordered!.Cast<T>().ToList();
    }

    private static List<T> SortLength<T>(this List<T> list, SortConfig<T> config)
    {
        var sorted = config.SortingMethod switch
        {
            SortingMethods.Reflection => SortingUtils.SortByLength(list, config),
            SortingMethods.Lambda => SortingUtils.SortByLength(list, config),
            _ => throw new ArgumentOutOfRangeException($"SortingMethod {config.SortingMethod}", "Not supported yet")
        };

        return sorted;
    }
}