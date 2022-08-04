using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.TPL
{
    [System.Serializable]
    public class TplTextureHashes
    {
        [field: SerializeField] public string[] TextureHashes { get; internal set; }

        public string this[int i] { get => TextureHashes[i]; }
    }
}
