using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace Teracura.AbstractSort.Logic;

public static class AbstractSorter
{
    public static void SortLength<T>(this List<T> list)
    {
        var sorted = list.OrderBy(item => item?.ToString()?.Length ?? -1)
            .ThenBy(item => item?.ToString()).ToList();
        list.Clear();
        list.AddRange(sorted);
    }
}