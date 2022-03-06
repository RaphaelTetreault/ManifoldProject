using GameCube.GFZ.Stage;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage
{
    /// <summary>
    /// 
    /// </summary>
    public class GfzCullOverrideTrigger : MonoBehaviour,
        IGfzConvertable<CullOverrideTrigger>
    {
        /// <summary>
        /// Unknown trigger scale (when compared to default Unity cube).
        /// </summary>
        public const float scale = 10f;

        // INSPECTOR FIELDS
        [SerializeField] private EnumFlags32 unk_0x20;
        //SerializeField] private EnumFlags16 unk_0x22;

        // PROPERTIES
        public EnumFlags32 Unk_0x20
        {
            get => unk_0x20;
            set => unk_0x20 = value;
        }
        //public EnumFlags16 Unk_0x22
        //{
        //    get => unk_0x22;
        //    set => unk_0x22 = value;
        //}


        // METHODS
        public CullOverrideTrigger ExportGfz()
        {
            // Convert unity transform to gfz transform
            var transform = TransformConverter.ToGfzTransformPRXS(this.transform);
            transform.Scale /= scale;

            var value = new CullOverrideTrigger
            {
                transform = transform,
                unk_0x20 = unk_0x20,
                //unk_0x22 = unk_0x22,
            };

            return value;
        }

        public void ImportGfz(CullOverrideTrigger value)
        {
            transform.CopyGfzTransformPRXS(value.transform);
            transform.localScale *= scale;
            unk_0x20 = value.unk_0x20;
            //unk_0x22 = value.unk_0x22;
        }

    }
}
