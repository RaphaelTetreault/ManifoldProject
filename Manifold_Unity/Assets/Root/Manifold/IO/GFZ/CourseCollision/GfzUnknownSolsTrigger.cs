using GameCube.GFZ.CourseCollision;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manifold.IO.GFZ.CourseCollision
{
    /// <summary>
    /// 
    /// </summary>
    public class GfzUnknownSolsTrigger : MonoBehaviour,
        IGfzConvertable<UnknownCollider>
    {
        /// <summary>
        /// Unknown SOLS trigger scale (when compared to default Unity cube).
        /// THIS IS 100% A GUESS.
        /// </summary>
        public const float scale = 27.5f;

        // INSPECTOR FIELDS
        [SerializeField] private int unk_0x00;

        // PROPERTIES
        public int Unk_0x00
        {
            get => unk_0x00;
            set => unk_0x00 = value;
        }

        // METHODS
        public UnknownCollider ExportGfz()
        {
            // Convert unity transform to gfz transform
            var transform = TransformConverter.ToGfzTransform(this.transform);

            var value = new UnknownCollider
            {
                unk_0x00 = unk_0x00,
                transform = transform
            };

            return value;
        }

        public void ImportGfz(UnknownCollider value)
        {
            transform.CopyGfzTransform(value.transform);
            transform.localScale *= scale;
            unk_0x00 = value.unk_0x00;
        }
    }
}
