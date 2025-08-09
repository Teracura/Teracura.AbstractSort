using Teracura.AbstractSort.Logic.Configurations;

namespace Teracura.AbstractSort.Logic.Sorting;

public static class AbstractSorter
{

    public static object Sort<T>(this List<T> list, SortConfig<T> config)
    {
        return config.SortMode switch
        {
            SortMode.Length => SortLength(list, config),
            SortMode.Version => SortVersion(list, config),
            _ => SortDefault(list, config)
        };
    }

    private static object SortDefault<T>(List<T> list, SortConfig<T> config)
    {
        throw new NotImplementedException();
    }

    private static object SortVersion<T>(List<T> list, SortConfig<T> config)
    {
        throw new NotImplementedException();
    }

    private static object SortLength<T>(this List<T> list, SortConfig<T> config)
    {

        var ascending = config.Ascending;
        var returnType = config.ReturnType;

        var sorted = config.SortingMethod switch
        {
            SortingMethods.Reflection => SortingUtils.SortByLength(list, config),
            SortingMethods.Lambda => SortingUtils.SortByLength(list, config),
            _ => throw new ArgumentOutOfRangeException($"SortingMethod {config.SortingMethod}", "Not supported yet")
        };

        if (!ascending)
            sorted.Reverse();

        list.Clear();
        list.AddRange(sorted);

        return SortingUtils.ReturnFromType(returnType, sorted);
    }
}