using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace Teracura.AbstractSort.Logic;

public static class AbstractSorter
{
    public static object SortLength<T>(this List<T> list, ReturnType returnType = ReturnType.List)
    {
        var sorted = list.OrderBy(item => item?.ToString()?.Length ?? -1)
            .ThenBy(item => item?.ToString()).ToList();
        list.Clear();
        list.AddRange(sorted);
        return returnType switch
        {
            ReturnType.List => sorted,
            ReturnType.Queue => new Queue<T>(sorted),
            ReturnType.Stack => new Stack<T>(sorted),
            ReturnType.HashSet => new HashSet<T>(sorted),
            _ => throw new ArgumentOutOfRangeException(nameof(returnType), $"Unknown return type: {returnType}")
        };
    }
}