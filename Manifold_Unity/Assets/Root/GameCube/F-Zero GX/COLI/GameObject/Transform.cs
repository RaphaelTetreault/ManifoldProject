using StarkTools.IO;
using System;
using System.IO;
using UnityEditor.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameCube.FZeroGX.COLI_COURSE
{
    [Serializable]
    public class Transform : IBinarySerializable, IBinaryAddressable
    {

        #region MEMBERS

        [SerializeField, Hex] long startAddress;
        [SerializeField, Hex] long endAddress;

        public Vector3 normalX;
        public float positionX;
        public Vector3 normalY;
        public float positionY;
        public Vector3 normalZ;
        public float positionZ;

        #endregion

        #region PROPERTIES

        public long StartAddress
        {
            get => startAddress;
            set => startAddress = value;
        }

        public long EndAddress
        {
            get => endAddress;
            set => endAddress = value;
        }

        //public Vector3 Position { get; private set; }
        //public Vector3 Scale { get; private set; }
        //public Quaternion Rotation { get; private set; }

        public Vector3 Position => new Vector3(positionX, positionY, positionZ);
        public Vector3 Scale => new Vector3(normalX.magnitude, normalY.magnitude, normalZ.magnitude);
        public Quaternion Rotation => SwapHandedness(Quaternion.LookRotation(normalZ, normalY));

        // Source: post #23
        // https://forum.unity.com/threads/right-hand-to-left-handed-conversions.80679/
        private Quaternion SwapHandedness(Quaternion input)
        {
            Vector3 rotation = input.eulerAngles;
            Vector3 flippedRotation = new Vector3(rotation.x, -rotation.y, -rotation.z); // flip Y and Z axis for right->left handed conversion
            // convert XYZ to ZYX
            Quaternion qx = Quaternion.AngleAxis(flippedRotation.x, Vector3.right);
            Quaternion qy = Quaternion.AngleAxis(flippedRotation.y, Vector3.up);
            Quaternion qz = Quaternion.AngleAxis(flippedRotation.z, Vector3.forward);
            Quaternion qq = qz * qy * qx; // this is the order
            return qq;
        }

        #endregion

        #region METHODS

        public void Deserialize(BinaryReader reader)
        {
            startAddress = reader.BaseStream.Position;

            reader.ReadX(ref normalX);
            reader.ReadX(ref positionX);
            reader.ReadX(ref normalY);
            reader.ReadX(ref positionY);
            reader.ReadX(ref normalZ);
            reader.ReadX(ref positionZ);

            endAddress = reader.BaseStream.Position;
        }

        public void Serialize(BinaryWriter writer)
        {
            throw new NotImplementedException();

            writer.WriteX(normalX);
            writer.WriteX(positionX);
            writer.WriteX(normalY);
            writer.WriteX(positionY);
            writer.WriteX(normalZ);
            writer.WriteX(positionZ);
        }

        public void SetUnityTransform(UnityEngine.Transform transform)
        {
            transform.localPosition = Position;
            transform.localScale = Scale;
            transform.rotation = Rotation;
        }

        #endregion

    }
}