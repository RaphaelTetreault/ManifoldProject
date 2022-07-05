using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.TPL
{
    [System.Serializable]
    public class TextureField
    {
        [field: SerializeField] public bool IsValid { get; internal set; }
        [field: SerializeField] public GameCube.GX.Texture.TextureFormat TextureFormat { get; internal set; }
        [field: SerializeField] public Texture2D Texture { get; internal set; }
    }
}
