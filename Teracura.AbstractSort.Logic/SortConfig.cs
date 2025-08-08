namespace Teracura.AbstractSort.Logic;

public class SortConfig
{
    public string Path { get; set; } = "";
    public bool UsePath { get; set; } = false;
    public bool Ascending { get; set; } = true;
    public ReturnType ReturnType { get; set; } = ReturnType.List;

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