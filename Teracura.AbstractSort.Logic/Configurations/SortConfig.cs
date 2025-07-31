using System.Reflection.Metadata;
using Teracura.AbstractSort.Logic.Enums;

namespace Teracura.AbstractSort.Logic.Configurations;

public class SortConfig<T>
{
    private bool _inParallel = false;
    private ReturnTypes _returnTypes = ReturnTypes.List;
    public readonly List<(Func<T, object> selector, bool ascending)> Criteria = new();

    public SortConfig<T> By<TKey>(Func<T,TKey> selector, bool ascending = true)
    {
        Criteria.Clear();
        Criteria.Add((x => selector(x), ascending));
        return this;
    }

    public SortConfig<T> InParallel(bool flag = true)
    {
        _inParallel = flag;
        return this;
    }
    
    public SortConfig<T> Returns(ReturnTypes returnType)
    {
        _returnTypes = returnType;
        return this;
    }
}