using System.Collections.Generic;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.TPL
{
    [CreateAssetMenu()]
    public class TplSobj : ScriptableObject
    {
        [field: SerializeField] public string SourceFileName { get; internal set; }
        [field: SerializeField] public TextureAndMipmaps[] TextureAndMipmaps { get; internal set; }

        public static TplSobj New(TextureAndMipmaps[] textureAndMipmaps)
        {
            var instance = CreateInstance<TplSobj>();
            instance.TextureAndMipmaps = textureAndMipmaps;
            return instance;
        }
    }
}
