namespace Manifold.IO
{
    public static partial class ConstRegex
    {
        public const string MatchIntegers = @"[0-9]+";
        public const string MatchFloat = @"[+-]?([0-9]*[.])?[0-9]+";
        public const string MatchWithinParenthesis = @"\(([^\)]+)\)";
    }
}
