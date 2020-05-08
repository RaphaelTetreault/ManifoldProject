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

        public Matrix4x4 matrix;

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

        #endregion

        #region METHODS

        public void Deserialize(BinaryReader reader)
        {
            Vector4 mtx0 = new Vector4();
            Vector4 mtx1 = new Vector4();
            Vector4 mtx2 = new Vector4();

            startAddress = reader.BaseStream.Position;

            reader.ReadX(ref mtx0);
            reader.ReadX(ref mtx1);
            reader.ReadX(ref mtx2);

            endAddress = reader.BaseStream.Position;

            matrix = new Matrix4x4(
                new Vector4(mtx0.x, mtx1.x, mtx2.x, 0),
                new Vector4(mtx0.y, mtx1.y, mtx2.y, 0),
                new Vector4(mtx0.z, mtx1.z, mtx2.z, 0),
                new Vector4(mtx0.w, mtx1.w, mtx2.w, 1)
                );

        }

        public void Serialize(BinaryWriter writer)
        {
            throw new NotImplementedException();
        }

        public void SetUnityTransform(UnityEngine.Transform transform)
        {
            transform.position = matrix.GetColumn(3);
            transform.localScale = matrix.lossyScale;
            transform.rotation = matrix.rotation;
        }

        #endregion

    }
}