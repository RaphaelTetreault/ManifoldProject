using GameCube.GFZ.Stage;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage
{
    public class GfzCullOverrideTrigger : MonoBehaviour,
        IGfzConvertable<CullOverrideTrigger>
    {
        /// <summary>
        /// Unknown trigger scale (when compared to default Unity cube).
        /// </summary>
        public const float scale = 10f;

        [field: SerializeField] public EnumFlags32 Unk_0x20 { get; private set; }


        public CullOverrideTrigger ExportGfz()
        {
            // Convert unity transform to gfz transform
            var transform = TransformConverter.ToGfzTransformTRXS(this.transform, Space.World);
            transform.Scale /= scale;

            var value = new CullOverrideTrigger
            {
                Transform = transform,
                Unk_0x20 = Unk_0x20,
            };

            return value;
        }

        public void ImportGfz(CullOverrideTrigger value)
        {
            transform.CopyGfzTransform(value.Transform, Space.Self);
            transform.localScale *= scale;
            Unk_0x20 = value.Unk_0x20;
        }

    }
}
