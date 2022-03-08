namespace Manifold.IO
{
    public static partial class Const
    {
        public static partial class Regex
        {
            public const string MatchIntegers = @"[0-9]+";
            public const string MatchFloat = @"[+-]?([0-9]*[.])?[0-9]+";
            public const string MatchWithinParenthesis = @"\(([^\)]+)\)";
        }
    }
}
