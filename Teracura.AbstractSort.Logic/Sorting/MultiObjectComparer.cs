namespace Teracura.AbstractSort.Logic.Sorting;

public class MultiObjectComparer : IComparer<object?>
{
    
    private static int TypeOrder(object? o) => o switch
    {
        null => 0,
        int or long or decimal or float or double => 1, // numbers first
        bool => 2,
        string => 3,
        DateTime => 4,
        Guid => 5,
        TimeSpan => 6,
        Array => 7,
        ValueTuple => 8,
        Delegate => 9,
        _ => 10
    };


    public int Compare(object? x, object? y)
    {
        if (ReferenceEquals(x, y)) return 0;
        if (x == null) return y == null ? 0 : -1;
        if (y == null) return 1;

        x = x switch
        {
            int ix => (double)ix,
            long lx => (double)lx,
            float fx => (double)fx,
            decimal decx => (double)decx,
            double dx => dx,
            _ => x
        };

        y = y switch
        {
            int iy => (double)iy,
            long ly => (double)ly,
            float fy => (double)fy,
            decimal decy => (double)decy,
            double dy => dy,
            _ => y
        };


        // Compare type precedence first
        var ox = TypeOrder(x);
        var oy = TypeOrder(y);
        if (ox != oy)
            return ox.CompareTo(oy);

        // Then compare actual values based on type
        switch (x)
        {
            case bool bx when y is bool by:
                return bx.CompareTo(by);
            case int ix when y is int iy:
                return ix.CompareTo(iy);
            case long lx when y is long ly:
                return lx.CompareTo(ly);
            case float fx when y is float fy:
                if (float.IsNaN(fx) && float.IsNaN(fy)) return 0;
                if (float.IsNaN(fx)) return 1;
                if (float.IsNaN(fy)) return -1;
                if (float.IsNegativeInfinity(fx) && float.IsNegativeInfinity(fy)) return 0;
                if (float.IsNegativeInfinity(fx)) return -1;
                if (float.IsNegativeInfinity(fy)) return 1;
                if (float.IsPositiveInfinity(fx) && double.IsPositiveInfinity(fy)) return 0;
                if (float.IsPositiveInfinity(fx)) return 1;
                if (float.IsPositiveInfinity(fy)) return -1;
                return fx.CompareTo(fy);
            case double dx when y is double dy:
                if (double.IsNaN(dx) && double.IsNaN(dy)) return 0;
                if (double.IsNaN(dx)) return 1;
                if (double.IsNaN(dy)) return -1;
                if (double.IsNegativeInfinity(dx) && double.IsNegativeInfinity(dy)) return 0;
                if (double.IsNegativeInfinity(dx)) return -1;
                if (double.IsNegativeInfinity(dy)) return 1;
                if (double.IsPositiveInfinity(dx) && double.IsPositiveInfinity(dy)) return 0;
                if (double.IsPositiveInfinity(dx)) return 1;
                if (double.IsPositiveInfinity(dy)) return -1;
                return dx.CompareTo(dy);
            case decimal decx when y is decimal decy:
                return decx.CompareTo(decy);
            case string sx when y is string sy:
                return string.Compare(sx, sy, StringComparison.Ordinal);
            case DateTime dtX when y is DateTime dtY:
                return dtX.CompareTo(dtY);
            case Guid gx when y is Guid gy:
                return gx.CompareTo(gy);
            case TimeSpan tsX when y is TimeSpan tsY:
                return tsX.CompareTo(tsY);
            case Array arrX when y is Array arrY:
                int lengthCompare = arrX.Length.CompareTo(arrY.Length);
                if (lengthCompare != 0) return lengthCompare;
                for (int i = 0; i < arrX.Length; i++)
                {
                    int cmp = Compare(arrX.GetValue(i), arrY.GetValue(i));
                    if (cmp != 0) return cmp;
                }
                return 0;
            case ValueTuple vtX when y is ValueTuple vtY:
                return string.Compare(vtX.ToString(), vtY.ToString(), StringComparison.Ordinal);
            case Delegate delX when y is Delegate delY:
                return string.Compare(delX.Method.Name, delY.Method.Name, StringComparison.Ordinal);
            default:
                return string.Compare(x.ToString(), y.ToString(), StringComparison.Ordinal);
        }
    }
}