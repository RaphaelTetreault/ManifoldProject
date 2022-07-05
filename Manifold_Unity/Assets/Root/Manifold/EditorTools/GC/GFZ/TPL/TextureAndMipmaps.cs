using Manifold.IO;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.TPL
{
    [System.Serializable]
    public class TextureAndMipmaps
    {
        [field: SerializeField] public TextureField[] TextureFields { get; internal set; }
        [field: SerializeField] public AddressRange SourceAddressRange { get; internal set; }
    }
}
