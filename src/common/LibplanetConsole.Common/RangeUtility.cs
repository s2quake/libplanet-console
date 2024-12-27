namespace LibplanetConsole.Common;

public static class RangeUtility
{
    public static class RangeParser
    {
        public static Range ParseRange(string rangeString)
        {
            if (rangeString == string.Empty)
            {
                throw new ArgumentException("Range string cannot be empty.", nameof(rangeString));
            }

            var parts = rangeString.Split("..");
            if (parts.Length != 2)
            {
                throw new FormatException("Invalid range format. Expected format is 'start..end'.");
            }

            var start = ParseIndex(parts[0]);
            var end = ParseIndex(parts[1]);

            return new Range(start, end);
        }

        private static Index ParseIndex(string indexString)
        {
            if (indexString == string.Empty)
            {
                throw new ArgumentException("Index string cannot be empty.", nameof(indexString));
            }

            if (indexString.StartsWith('^') is true)
            {
                var value = int.Parse(indexString[1..]);
                return new Index(value, fromEnd: true);
            }
            else
            {
                var value = int.Parse(indexString);
                return new Index(value, fromEnd: false);
            }
        }
    }
}
