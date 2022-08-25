using GameCube.GFZ;
using GameCube.GFZ.Stage;
using UnityEngine;


namespace Manifold.EditorTools.GC.GFZ
{
    public static class TransformConverter
    {
        public static Vector3 MirrorPosition(Vector3 position)
        {
            position.z = -position.z;
            return position;
        }
        public static Vector3 MirrorNormal(Vector3 normal) => MirrorPosition(normal);
        public static Vector3 MirrorRotation(Vector3 rotation)
        {
            rotation.x = -rotation.x;
            rotation.y = -rotation.y;
            return rotation;
        }
        public static Quaternion MirrorRotation(Quaternion rotation)
        {
            rotation.x = -rotation.x;
            rotation.y = -rotation.y;
            return rotation;
        }
        public static Vector3 ApplyParentScale(Vector3 scale, Transform parent)
        {
            Vector3 parentScale = parent.lossyScale;
            scale.x /= parentScale.x;
            scale.y /= parentScale.y;
            scale.z /= parentScale.z;
            return scale;
        }


        public static Matrix4x4 ToUnity(TransformTRXS trxs)
        {
            var position = MirrorPosition(trxs.Position);
            var scale = trxs.Scale;

            // HACK: you need to fix the BS going on with compressed rotations...
            var qx = Quaternion.Euler(new Vector3(trxs.RotationEuler.x, 0, 0));
            var qy = Quaternion.Euler(new Vector3(0, trxs.RotationEuler.y, 0));
            var qz = Quaternion.Euler(new Vector3(0, 0, trxs.RotationEuler.z));
            var rotation = qz * qy * qx;
            rotation = MirrorRotation(rotation);

            var matrix = Matrix4x4.TRS(position, rotation, scale);
            return matrix;
        }
        public static Matrix4x4 ToUnity(TransformMatrix3x4 mtx3x4)
        {
            var position = MirrorPosition(mtx3x4.Position);
            var rotation = MirrorRotation(mtx3x4.Matrix.ToUnityMatrix4x4().rotation); // 'quaternion' is... lacking
            var scale = mtx3x4.Scale;
            var matrix = Matrix4x4.TRS(position, rotation, scale);
            return matrix;
        }

        public static void CopyTransform(this Transform to, TransformTRXS from)
        {
            var matrix = ToUnity(from);
            to.localPosition = matrix.Position();
            to.localRotation = matrix.rotation;
            to.localScale = matrix.lossyScale;
        }
        public static void CopyTransform(this Transform to, TransformMatrix3x4 from)
        {
            var matrix = ToUnity(from);
            to.localPosition = matrix.Position();
            to.localRotation = matrix.rotation;
            to.localScale = matrix.lossyScale;
        }

        public static void CopyTransform(this TransformTRXS to, Transform from, Space space)
        {
            bool isLocal = space == Space.Self;
            // Get local or global
            Vector3 position = isLocal ? from.localPosition : from.position;
            Quaternion rotation = isLocal ? from.localRotation : from.rotation;
            Vector3 scale = isLocal ? from.localScale : from.lossyScale;
            // Mirror
            position = MirrorPosition(position);
            rotation = MirrorRotation(rotation);
            // Special rotation
            CompressedRotation compressedRotation = new CompressedRotation()
            {
                Eulers = rotation.eulerAngles
            };
            // Set
            to.Position = position;
            to.CompressedRotation = compressedRotation;
            to.Scale = scale;
        }
        public static void CopyTransform(this TransformMatrix3x4 to, Transform from, Space space)
        {
            bool isLocal = space == Space.Self;
            // Get local or global
            Vector3 position = isLocal ? from.localPosition : from.position;
            Quaternion rotation = isLocal ? from.localRotation : from.rotation;
            Vector3 scale = isLocal ? from.localScale : from.lossyScale;
            // Mirror
            position = MirrorPosition(position);
            rotation = MirrorRotation(rotation);
            // Set
            var matrix = Matrix4x4.TRS(position, rotation, scale);
            to.Matrix = matrix;
        }

        public static TransformTRXS ToGfzTransformTRXS(Transform unity, Space space)
        {
            var gfz = new TransformTRXS();
            CopyTransform(gfz, unity, space);
            return gfz;
        }

        public static TransformMatrix3x4 ToGfzTransformMatrix3x4(Transform unity, Space space)
        {
            var gfz = new TransformMatrix3x4();
            CopyTransform(gfz, unity, space);
            return gfz;
        }

    }
}
