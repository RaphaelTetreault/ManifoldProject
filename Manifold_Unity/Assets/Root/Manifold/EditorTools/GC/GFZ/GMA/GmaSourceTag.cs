using Manifold.IO;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.GMA
{
    public sealed class GmaSourceTag : MonoBehaviour
    {
        [field: SerializeField, ReadOnlyGUI] public string FileName { get; set; }
        [field: SerializeField, ReadOnlyGUI] public string ModelName { get; set; }
        [field: SerializeField, ReadOnlyGUI] public AddressRange GcmfAddressRange { get; set; }
    }
}
