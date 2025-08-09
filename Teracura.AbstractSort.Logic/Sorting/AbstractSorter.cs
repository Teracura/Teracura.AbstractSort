using Teracura.AbstractSort.Logic.Configurations;

namespace Teracura.AbstractSort.Logic.Sorting;

public static class AbstractSorter
{
    public static object SortLength<T>(this List<T> list, SortConfig<T>? config = null)
    {
        config ??= new SortConfig<T>.Builder().Build();

        var ascending = config.Ascending;
        var returnType = config.ReturnType;

        var sorted = config.SortingMethod switch
        {
            SortingMethods.Reflection => SortingUtils.SortByLengthReflection(list, config.ReflectionPaths),
            SortingMethods.Lambda => SortingUtils.SortByLengthLambda(list, config.LambdaSelectors),
            _ => throw new ArgumentOutOfRangeException($"SortingMethod {config.SortingMethod}", "Not supported yet")
        };

        if (!ascending)
            sorted.Reverse();

        list.Clear();
        list.AddRange(sorted);

        return SortingUtils.ReturnFromType(returnType, sorted);
    }
}