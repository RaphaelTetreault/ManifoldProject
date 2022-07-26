using GameCube.GFZ;
using GameCube.GFZ.Stage;
using Manifold.IO;
using UnityEngine;


namespace Manifold.EditorTools.GC.GFZ
{
    public static class TransformConverter
    {
        // EXTENSIONS
        public static void CopyGfzTransform(this Transform unityTransform, TransformTRXS gfzTransform, Space space)
        {
            ToUnity(gfzTransform, unityTransform, space);
        }
        public static void CopyGfzTransform(this Transform unityTransform, TransformMatrix3x4 gfzTransform, Space space)
        {
            ToUnity(gfzTransform, unityTransform, space);
        }

        public static void CopyUnityTransform(this TransformTRXS gfzTransform, Transform unityTransform, Space space)
        {
            ToGfz(unityTransform, gfzTransform, space);
        }
        public static void CopyUnityTransform(this TransformMatrix3x4 gfzTransform, Transform unityTransform, Space space)
        {
            ToGfz(unityTransform, gfzTransform, space);
        }

        public static void ToUnity(TransformTRXS from, Transform to, Space space)
        {
            // Get TRS
            var position = from.Position;
            var rotation = from.Rotation;
            var scale = from.Scale;

            // Convert right-to-left handedness
            position.z = -position.z;
            var eulerRotation = ((Quaternion)rotation).eulerAngles;
            eulerRotation.x = -eulerRotation.x;
            eulerRotation.y = -eulerRotation.y;
            rotation = Quaternion.Euler(eulerRotation);

            // Apply transform values to target
            bool useLocal = space == Space.Self;
            bool hasParent = to.parent != null;
            if (useLocal || hasParent)
            {
                to.localPosition = position;
                to.localRotation = rotation;
                to.localScale = scale;
            }
            else
            {
                // If we have parent, make "true" world space scale
                var parentScale = to.parent.lossyScale;
                scale /= parentScale;

                to.position = position;
                to.rotation = rotation;
                to.localScale = scale;
            }
        }
        public static void ToUnity(TransformMatrix3x4 from, Transform to, Space space)
        {
            // Get TRS
            var position = from.Position;
            var rotation = from.Rotation;
            var scale = from.Scale;

            // Convert right-to-left handedness
            position.z = -position.z;
            var eulerRotation = ((Quaternion)rotation).eulerAngles;
            eulerRotation.x = -eulerRotation.x;
            eulerRotation.y = -eulerRotation.y;
            rotation = Quaternion.Euler(eulerRotation);

            // Apply transform values to target
            bool useLocal = space == Space.Self;
            bool hasParent = to.parent != null;
            if (useLocal || hasParent)
            {
                to.localPosition = position;
                to.localRotation = rotation;
                to.localScale = scale;
            }
            else
            {
                // If we have parent, make "true" world space scale
                var parentScale = to.parent.lossyScale;
                scale /= parentScale;

                to.position = position;
                to.rotation = rotation;
                to.localScale = scale;
            }
        }

        public static void ToGfz(Transform unity, TransformTRXS gfz, Space space)
        {
            // Get TRS
            bool useLocal = space == Space.Self;
            Vector3 position = useLocal ? unity.localPosition : unity.position;
            Vector3 rotation = useLocal ? unity.localRotation.eulerAngles : unity.rotation.eulerAngles;
            Vector3 scale = useLocal ? unity.localScale : unity.lossyScale;

            // Coonvert coordinate spaces
            position.z = -position.z;
            rotation.x = -rotation.x;
            rotation.y = -rotation.y;

            // Set values
            gfz.Position = position;
            gfz.CompressedRotation = new CompressedRotation() { Eulers = rotation };
            gfz.Scale = scale;
        }
        public static void ToGfz(Transform unity, TransformMatrix3x4 gfz, Space space)
        {
            // Get TRS
            bool useLocal = space == Space.Self;
            Vector3 position = useLocal ? unity.localPosition : unity.position;
            Vector3 rotation = useLocal ? unity.localRotation.eulerAngles : unity.rotation.eulerAngles;
            Vector3 scale = useLocal ? unity.localScale : unity.lossyScale;

            // Coonvert coordinate spaces
            position.z = -position.z;
            rotation.x = -rotation.x;
            rotation.y = -rotation.y;

            // Make matrix
            var matrix = new Matrix4x4();
            matrix.SetTRS(position, Quaternion.Euler(rotation), scale);

            // Set value to transform, implicitely converts Matrix4x4 to float4x4
            gfz.Matrix = matrix;
        }

        public static TransformTRXS ToGfzTransformTRXS(Transform unityTransform, Space space)
        {
            var trxs = new TransformTRXS();
            trxs.CopyUnityTransform(unityTransform, space);
            return trxs;
        }
        public static TransformMatrix3x4 ToGfzTransformMatrix3x4(Transform unityTransform, Space space)
        {
            var value = new TransformMatrix3x4();
            value.CopyUnityTransform(unityTransform, space);
            return value;
        }

    }
}
