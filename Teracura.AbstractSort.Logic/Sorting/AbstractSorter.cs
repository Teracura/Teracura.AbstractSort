using Teracura.AbstractSort.Logic.Configurations;

namespace Teracura.AbstractSort.Logic.Sorting;

public static class AbstractSorter
{
    public static object Sort<T>(this List<T> list, SortConfig<T> config)
    {
        var sorted = config.SortMode switch
        {
            SortMode.Length => SortLength(list, config),
            SortMode.Version => SortVersion(list, config),
            _ => SortDefault(list, config)
        };

        if (!config.Ascending)
            sorted.Reverse();
        if (config.MutateOriginal)
        {
            list.Clear();
            list.AddRange(sorted);
        }

        return SortingUtils.ReturnFromType(config.ReturnType, sorted);
    }

    private static List<T> SortDefault<T>(List<T> list, SortConfig<T> config)
    {
        list.Sort();
        return list;
    }

    private static List<T> SortVersion<T>(List<T> list, SortConfig<T> config)
    {
        if (typeof(T) != typeof(string))
            throw new ArgumentException("Sorting by version only supports strings");

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