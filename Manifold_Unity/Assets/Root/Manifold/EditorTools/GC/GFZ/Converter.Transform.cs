using GameCube.GFZ;
using GameCube.GFZ.Stage;
using Manifold.IO;
using UnityEngine;


namespace Manifold.EditorTools.GC.GFZ
{
    public static class TransformConverter
    {
        // EXTENSIONS
        public static void CopyGfzTransformPRXS(this Transform unityTransform, TransformPRXS gfzTransform)
        {
            CopyToUnityTransform(gfzTransform, unityTransform);
        }
        public static void CopyGfzTransformMatrix3x4(this Transform unityTransform, TransformMatrix3x4 gfzTransform)
        {
            CopyToUnityTransform(gfzTransform, unityTransform);
        }

        public static void CopyUnityTransform(this TransformPRXS gfzTransform, Transform unityTransform)
        {
            CopyToGfzTransformPRXS(unityTransform, gfzTransform);
        }
        public static void CopyUnityTransform(this TransformMatrix3x4 gfzTransform, Transform unityTransform)
        {
            CopyToGfzTransformMatrix3x4(unityTransform, gfzTransform);
        }


        // HELPERS

        /// <summary>
        /// Copies the TRS values from GFZ Transform <paramref name="from"/> to the Unity Transform <paramref name="to"/>
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public static void CopyToUnityTransform(TransformPRXS from, Transform to)
        {
            // Apply transform values to target
            to.localPosition = from.Position;
            to.localRotation = from.Rotation;
            to.localScale = from.Scale;
        }

        /// <summary>
        /// Copies the matrix from the TransformMatrix3x4  <paramref name="from"/> to the Unity Transform <paramref name="to"/>
        /// </summary>
        /// <param name="from">The transform to copy local TRS from.</param>
        /// <param name="to">The transform to apply TRS to.</param>
        public static void CopyToUnityTransform(TransformMatrix3x4 from, Transform to)
        {
            // Apply transform values to target
            to.localPosition = from.Position;
            //to.rotation = from.Rotation; // 'quaternion' is... lacking
            to.localRotation = from.Matrix.ToUnityMatrix4x4().rotation;
            to.localScale = from.Scale;
        }


        /// <summary>
        /// Copies the (global) TRS values from the Unity Transform <paramref name="unity"/> to the GFZ Transform <paramref name="gfzPRXS"/>.
        /// </summary>
        /// <param name="unity">The transform to copy global TRS from.</param>
        /// <param name="gfzPRXS">The transform to apply global TRS to.</param>
        public static void CopyToGfzTransformPRXS(Transform unity, TransformPRXS gfzPRXS)
        {
            // Copy over GLOBAL position.
            // The game does uses "TransformMatrix3x4" for LOCAL coordinates.
            gfzPRXS.Position = unity.position;
            gfzPRXS.DecomposedRotation = unity.rotation.eulerAngles;
            gfzPRXS.Scale = unity.lossyScale;
        }

        /// <summary>
        /// Copies the matrix from the Unity Transform <paramref name="from"/> to the GFX TransformMatrix <paramref name="to"/>.
        /// </summary>
        /// <param name="from">The transform to copy local TRS from.</param>
        /// <param name="to">The transform to apply local TRS to.</param>
        public static void CopyToGfzTransformMatrix3x4(Transform from, TransformMatrix3x4 to)
        {
            // Create Unity Matrix for easy setup of TRS.
            // Use LOCAL coordinates since this structure may exist in a hierarchy with parenting.
            var matrix = new Matrix4x4();
            matrix.SetTRS(
                from.localPosition,
                from.localRotation,
                from.localScale);

            // Set value to transform, implicitely converts Matrix4x4 to float4x4
            to.Matrix = matrix;
        }

        /// <summary>
        /// Returns a new GFZ Transform with the global coordinates of the <paramref name="unityTransform"/>
        /// </summary>
        /// <param name="unityTransform">The transform to copy global TRS from.</param>
        /// <returns></returns>
        public static TransformPRXS ToGfzTransformPRXS(Transform unityTransform)
        {
            var value = new TransformPRXS();
            value.CopyUnityTransform(unityTransform);

            return value;
        }

        /// <summary>
        /// Returns a new TransformMatrix3x4 with the local coordinates of the <paramref name="unityTransform"/>
        /// </summary>
        /// <param name="unityTransform">The transform to copy local TRS from.</param>
        /// <returns></returns>
        public static TransformMatrix3x4 ToGfzTransformMatrix3x4(Transform unityTransform)
        {
            var value = new TransformMatrix3x4();
            value.CopyUnityTransform(unityTransform);

            return value;
        }

    }
}
