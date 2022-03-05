using Manifold.IO;
using System.IO;
using UnityEngine;

namespace GameCube.GFZ
{
    // https://nghiaho.com/?page_id=846

    [System.Serializable]
    public struct Int16Rotation3 :
        IBinarySerializable
    {
        public Int16Rotation phi;
        public Int16Rotation theta;
        public Int16Rotation psi;

        private Quaternion rotation;
        private Vector3 eulerAngles;

        public Vector3 EulerAngles => eulerAngles;
        public Quaternion Rotation => rotation;


        public static implicit operator Quaternion(Int16Rotation3 value)
        {
            return value.rotation;
        }

        public static implicit operator Vector3(Int16Rotation3 value)
        {
            return value.eulerAngles;
        }

        public static implicit operator Int16Rotation3(Vector3 eulerAngles)
        {
            // Convert euler angles to matrix
            Matrix4x4 matrix = new Matrix4x4();
            matrix.SetTRS(Vector3.zero, Quaternion.Euler(eulerAngles), Vector3.one);
            
            // Decompose matrix
            var (phi, theta, psi) = DecomposeRotation(matrix);

            // Assign values
            var ushortRotation = new Int16Rotation3()
            {
                phi = phi,
                theta = theta,
                psi = psi,

                eulerAngles = eulerAngles,
                rotation = Quaternion.Euler(eulerAngles),
            };
            return ushortRotation;
        }

        public static implicit operator Int16Rotation3(Quaternion rotation)
        {
            // Convert euler angles to matrix
            Matrix4x4 matrix = new Matrix4x4();
            matrix.SetTRS(Vector3.zero, rotation, Vector3.one);

            // Decompose matrix
            var (phi, theta, psi) = DecomposeRotation(matrix);

            // Assign values
            var ushortRotation = new Int16Rotation3()
            {
                phi = phi,
                theta = theta,
                psi = psi,

                eulerAngles = rotation.eulerAngles,
                rotation = rotation,
            };
            return ushortRotation;
        }

        public static Quaternion RecomposeRotation(float phi, float theta, float psi)
        {
            // Reconstruct rotation from partial data
            var rotation = Quaternion.identity;
            // Apply rotation in discrete sequence. Yes, really.
            rotation = Quaternion.Euler(phi, 0, 0) * rotation;
            rotation = Quaternion.Euler(0, theta, 0) * rotation;
            rotation = Quaternion.Euler(0, 0, psi) * rotation;

            return rotation;
        }

        public static (float phi, float theta, float psi) DecomposeRotation(Matrix4x4 matrix)
        {
            // Decompose rotation matrix into 3 rotations
            // https://nghiaho.com/?page_id=846
            var r11 = matrix.m00;
            var r21 = matrix.m10;
            var r31 = matrix.m20;
            var r32 = matrix.m21;
            var r33 = matrix.m22;

            // Compute discrete rotation steps
            var phi = Mathf.Atan2(r32, r33);
            var theta = Mathf.Atan2(-r31, Mathf.Sqrt(Mathf.Pow(r32, 2) + Mathf.Pow(r33, 2)));
            var psi = Mathf.Atan2(r21, r11);

            // Convert to degrees
            phi     *= Mathf.Rad2Deg;
            theta   *= Mathf.Rad2Deg;
            psi     *= Mathf.Rad2Deg;

            return (phi, theta, psi);
        }

        public void Deserialize(BinaryReader reader)
        {
            {
                reader.ReadX(ref phi);
                reader.ReadX(ref theta);
                reader.ReadX(ref psi);
            }
            //
            {
                // Reconstruct rotation from partial data
                rotation = RecomposeRotation(phi, theta, psi);
                eulerAngles = rotation.eulerAngles;
            }
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.WriteX(phi);
            writer.WriteX(theta);
            writer.WriteX(psi);
        }

        public override string ToString()
        {
            var euler = rotation.eulerAngles;
            return $"({euler.x:0.0}, {euler.x:0.0}, {euler.x:0.0})";
        }
    }
}
