namespace Manifold.IO.GFZ
{
    public static partial class Const
    {
        public static partial class RegionCode
        {
            public const string GFZJ8P = "GFZJ8P"; // AX
            public const string GFZJ01 = "GFZJ01"; // GX Japanese
            public const string GFZE01 = "GFZE01"; // GX English
            public const string GFZP01 = "GFZP01"; // GX Europe

            public static readonly string[] allRegions = new string[]
            {
                GFZJ8P,
                GFZJ01,
                GFZE01,
                GFZP01
            };
        }
    }
}
