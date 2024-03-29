﻿using Manifold.IO;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.TPL
{
    [System.Serializable]
    public class TextureInfo
    {
        [field: SerializeField] public string SourceFileName { get; internal set; }
        [field: SerializeField] public AddressRange AddressRange { get; internal set; }
        [field: SerializeField] public GameCube.GX.Texture.TextureFormat TextureFormat { get; internal set; }
        [field: SerializeField] public ushort PixelWidth { get; internal set; }
        [field: SerializeField] public ushort PixelHeight { get; internal set; }
        [field: SerializeField] public ushort TextureLevels { get; internal set; }

        public GameCube.GFZ.TPL.TextureDescription AsTextureDescription()
        {
            var description = new GameCube.GFZ.TPL.TextureDescription()
            {
                TextureFormat = TextureFormat,
                Width = PixelWidth,
                Height = PixelHeight,
                IsNull = false,
                MipmapLevels = TextureLevels,
                AddressRange = AddressRange,
            };
            return description;
        }
    }
}
