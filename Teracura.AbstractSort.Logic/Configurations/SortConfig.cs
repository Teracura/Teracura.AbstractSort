namespace Teracura.AbstractSort.Logic.Configurations;

public class SortConfig<T>
{
    public List<string> ReflectionPaths { get; private set; } = [];
    public List<Func<T, object?>?> LambdaSelectors { get; private set; } = [];
    public SortingMethods SortingMethod { get; private set; } = SortingMethods.Reflection;
    public SortMode SortMode { get; private set; } = SortMode.None;
    public bool Ascending { get; private set; } = true;
    public bool CaseSensitive { get; private set; } = true;
    public ReturnType ReturnType { get; private set; } = ReturnType.List;


    private SortConfig()
    {
    }

    public class Builder
    {
        private readonly SortConfig<T> _config = new();
        private bool _usedSortBy;

        public Builder SortBy(string path)
        {
            _config.SortingMethod = SortingMethods.Reflection;

            if (_config.ReflectionPaths.Count == 0)
            {
                _config.ReflectionPaths.Add(path);
                _usedSortBy = true;
                return this;
            }

            _config.ReflectionPaths[0] = path;
            _usedSortBy = true;
            return this;
        }

        public Builder SortBy(Func<T, object?>? expression)
        {
            _config.SortingMethod = SortingMethods.Lambda;
            if (_config.LambdaSelectors.Count == 0)
            {
                _config.LambdaSelectors.Add(expression);
                _usedSortBy = true;
                return this;
            }

            _config.LambdaSelectors[0] = expression;
            _usedSortBy = true;
            return this;
        }

        public Builder ThenBy(string path)
        {
            if (!_usedSortBy)
                throw new InvalidOperationException("ThenBy must follow a SortBy");

            if (_config.SortingMethod != SortingMethods.Reflection)
            {
                throw new InvalidOperationException("Cannot use different sorting methods on the same SortConfig");
            }

            _config.ReflectionPaths.Add(path);
            return this;
        }

        public Builder ThenBy(Func<T, object?>? expression)
        {
            if (!_usedSortBy)
            {
                throw new InvalidOperationException("ThenBy must follow a SortBy");
            }

            if (_config.SortingMethod != SortingMethods.Lambda)
            {
                throw new InvalidOperationException("Cannot use different sorting methods on the same SortConfig");
            }

            _config.LambdaSelectors.Add(expression);
            return this;
        }

        public Builder Mode(SortMode mode)
        {
            _config.SortMode = mode;
            return this;
        }

        public Builder Ascending(bool ascending = true)
        {
            _config.Ascending = ascending;
            return this;
        }

        public Builder CaseSensitive(bool caseSensitive = true)
        {
            _config.CaseSensitive = caseSensitive;
            return this;
        }

        public Builder ReturnType(ReturnType type = Configurations.ReturnType.List)
        {
            _config.ReturnType = type;
            return this;
        }

        public SortConfig<T> Build()
        {
            return _config;
        }
    }
}