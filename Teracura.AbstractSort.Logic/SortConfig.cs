namespace Teracura.AbstractSort.Logic;

public class SortConfig
{
    public string Path { get; private set; } = "";
    public bool UsePath { get; private set; } = false;
    public bool Ascending { get; private set; } = true;
    public ReturnType ReturnType { get; private set; } = ReturnType.List;

    private SortConfig()
    {
    }

    public class Builder
    {
        private readonly SortConfig _config = new();

        public Builder UsePropertyPath(bool usePropertyPath = true)
        {
            _config.UsePath = usePropertyPath;
            return this;
        }

        public Builder SetPropertyPath(string path)
        {
            _config.Path = path;
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

        public SortConfig Build()
        {
            return _config;
        }
    }
}