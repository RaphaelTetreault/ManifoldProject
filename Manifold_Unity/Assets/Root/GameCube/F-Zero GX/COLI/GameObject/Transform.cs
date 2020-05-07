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

        public Vector3 Position { get; private set; }
        public Vector3 Scale { get; private set; }
        public Quaternion Rotation { get; private set; }

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

            Position = new Vector3(positionX, positionY, positionZ);
            Scale = new Vector3(normalX.magnitude, normalY.magnitude, normalZ.magnitude);
            Rotation = Quaternion.LookRotation(normalZ, normalY); // this is wrong
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