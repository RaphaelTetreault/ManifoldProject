using GameCube.GFZ.GMA;
using GameCube.GFZ.TPL;
using Manifold.EditorTools.GC.GFZ;
using Manifold.EditorTools.GC.GFZ.TPL;
using System.Collections;
using System.Collections.Generic;

namespace Manifold.EditorTools.GC.GFZ
{
    public static partial class GfzAssetTemplates
    {
        public static class GmaMaterials
        {
            public static Material MuteCityTrackTop(Dictionary<string, short> hashesToTextureIndex)
            {
                var texture0 = hashesToTextureIndex["00c7c57df7d4600f51bffd995efa9605"];
                Material material = new Material()
                {
                    MaterialColor = new GameCube.GX.GXColor(),
                    AmbientColor = new GameCube.GX.GXColor(),
                    SpecularColor = new GameCube.GX.GXColor(),
                    Unk0x10 = (MatFlags0x10)0,
                    Alpha = 255,
                    TextureCount = 1,
                    DisplayListFlags = DisplayListFlags.PrimaryFrontCull,
                    Unk0x14 = 0,
                    Unk0x15 = (MatFlags0x15)0,
                    TextureIndex0 = texture0,
                    TextureIndex1 = -1,
                    TextureIndex2 = -1,
                };
                return material;
            }
        }
    }
}
