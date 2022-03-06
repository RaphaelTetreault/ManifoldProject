using Manifold.IO;
using System.IO;
using Unity.Mathematics;
using static Unity.Mathematics.math;

namespace GameCube.GFZ
{
    // Resource that got me through this:
    // https://nghiaho.com/?page_id=846

    /// <summary>
    /// Amusement Vision's oddball rotation struct. Rather than store 3 floats for XYZ, they
    /// serialize 3 int16 values normalized. Conceptually, these int16 represent -pi to +pi.
    /// Moreover, this rotation "raw" is decomposed, meaning that the values are not suitable
    /// as-is and must be recomposed in a matrix (reconstructed) to be a valid rotation.
    /// </summary>
    [System.Serializable]
    public struct CompressedRotation :
        IBinarySerializable
    {
        // METADATA
        private quaternion quaternion;
        private float3 eulers;


        // FIELDS
        private Int16Rotation x; // phi
        private Int16Rotation y; // theta
        private Int16Rotation z; // psi


        // PROPERTIES
        public Int16Rotation X
        {
            get => x;
            set
            {
                x = value;
                ComputeProperties();
            }
        }
        public Int16Rotation Y
        {
            get => y;
            set
            {
                y = value;
                ComputeProperties();
            }
        }
        public Int16Rotation Z
        {
            get => z;
            set
            {
                z = value;
                ComputeProperties();
            }
        }
        public quaternion Quaternion
        {
            get => quaternion;
            // TODO: implement when Unity.Mathematics supports quaternion <=> eulers
            // set {}
        }
        public float3 Eulers
        {
            get => eulers;
            set
            {
                x = eulers.x;
                y = eulers.y;
                z = eulers.z;
                ComputeProperties();
            }
        }


        // METHODS

        /// <summary>
        /// Reconstruct the rotation
        /// </summary>
        /// <param name="xRadians"></param>
        /// <param name="yRadians"></param>
        /// <param name="zRadians"></param>
        /// <returns></returns>
        public static quaternion RecomposeRotation(float xRadians, float yRadians, float zRadians)
        {
            // Reconstruct componenets as matrices 3x3
            var mtxX = new float3x3(
                1, 0, 0,
                0, cos(xRadians), -sin(xRadians),
                0, sin(xRadians), cos(xRadians)
                );
            var mtxY = new float3x3(
                cos(yRadians), 0, sin(yRadians),
                0, 1, 0,
                -sin(yRadians), 0, cos(yRadians)
                );
            var mtxZ = new float3x3(
                cos(zRadians), -sin(zRadians), 0,
                sin(zRadians), cos(zRadians), 0,
                0, 0, 1
                );

            // 
            var mtx = mtxZ * mtxY * mtxX;
            var rotation = new quaternion(mtx);

            return rotation;
        }

        public static float3 DecomposeRotationDegrees(float3x3 matrix)
        {
            // Get the relevant parts of the rotation from the matrix
            // https://nghiaho.com/?page_id=846
            float r11 = matrix.c0.x;
            float r21 = matrix.c1.y;
            float r31 = matrix.c2.x;
            float r32 = matrix.c2.y;
            float r33 = matrix.c2.z;

            // Compute discrete rotation steps
            float xRadians = atan2(r32, r33);
            float yRadians = atan2(-r31, sqrt(pow(r32, 2) + pow(r33, 2)));
            float zRadians = atan2(r21, r11);

            // Set angles to be in degrees, not radians
            float3 decomposedEulers = degrees(new float3(xRadians, yRadians, zRadians));
            return decomposedEulers;
        }

        public void Deserialize(BinaryReader reader)
        {
            reader.ReadX(ref x);
            reader.ReadX(ref y);
            reader.ReadX(ref z);

            // Initializes quaternion and euler properties
            ComputeProperties();
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.WriteX(x);
            writer.WriteX(y);
            writer.WriteX(z);
        }

        private void ComputeProperties()
        {
            quaternion = RecomposeRotation(x.Radians, y.Radians, z.Radians);
            eulers = new float3(x.Degrees, y.Degrees, z.Degrees);
        }

        public override string ToString()
        {
            return $"({eulers.x:0.0}, {eulers.y:0.0}, {eulers.z:0.0})";
        }
    }
}
