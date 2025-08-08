namespace Teracura.AbstractSort.Logic;

public class SortConfig<T>
{
    public List<string> ReflectionPaths { get; private set; } = [];
    public List<Func<T, object?>?> LambdaSelectors { get; private set; } = [];
    public bool UseReflectionPath { get; private set; } = true;
    public bool Ascending { get; private set; } = true;
    public bool UsePropertyExpression { get; private set; } = false;
    public ReturnType ReturnType { get; private set; } = ReturnType.List;


    private SortConfig()
    {
    }

    public class Builder
    {
        private readonly SortConfig<T> _config = new();
        private bool _usedSortBy = false;

        public Builder SortBy(string path)
        {
            _config.UseReflectionPath = true;
            _config.UsePropertyExpression = false;

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
            _config.UsePropertyExpression = true;
            _config.UseReflectionPath = false;
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

            if (!_config.UseReflectionPath)
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

            if (!_config.UsePropertyExpression)
            {
                throw new InvalidOperationException("Cannot use different sorting methods on the same SortConfig");
            }

            _config.LambdaSelectors.Add(expression);
            return this;
        }

        public Builder SortAscending(bool ascending = true)
        {
            _config.Ascending = ascending;
            return this;
        }

        public Builder ReturnType(ReturnType type = Logic.ReturnType.List)
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