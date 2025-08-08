namespace Teracura.AbstractSort.Logic;

public class SortConfig<T>
{
    public string Path { get; private set; } = "";
    public bool UseReflectionPath { get; private set; } = true;
    public bool Ascending { get; private set; } = true;
    public bool UsePropertyExpression { get; private set; } = false;
    public Func<T, object?>? ExpressionLambda { get; private set; } = null;
    public ReturnType ReturnType { get; private set; } = ReturnType.List;
    

    private SortConfig()
    {
    }

    public class Builder
    {
        private readonly SortConfig<T> _config = new();

        public Builder SortBy(string path)
        {
            _config.UseReflectionPath = true;
            _config.UsePropertyExpression = false;
            _config.Path = path;
            return this;
        }

        public Builder SortBy(Func<T, object?>? expression)
        {
            _config.UsePropertyExpression = true;
            _config.UseReflectionPath = false;
            _config.ExpressionLambda = expression;
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