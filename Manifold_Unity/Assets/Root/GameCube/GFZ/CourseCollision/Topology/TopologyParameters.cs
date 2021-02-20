using Manifold.IO;
using System;
using System.IO;
using UnityEngine;

namespace GameCube.GFZ
{
    [Serializable]
    public class TopologyParameters : IBinarySerializable, IBinaryAddressable
    {
        public const int kFieldCount = 9;

        #region MEMBERS

        [SerializeField, Hex] long startAddress;
        [SerializeField, Hex] long endAddress;

        //public int[] counts;
        //public int[] absPtrs;

        [Hex(8), Space]
        public int count1;
        [Hex(8)]
        public int count2;
        [Hex(8)]
        public int count3;
        [Hex(8)]
        public int count4;
        [Hex(8)]
        public int count5;
        [Hex(8)]
        public int count6;
        [Hex(8)]
        public int count7;
        [Hex(8)]
        public int count8;
        [Hex(8)]
        public int count9;
        [Hex(8)]
        public int absPtr1;
        [Hex(8)]
        public int absPtr2;
        [Hex(8)]
        public int absPtr3;
        [Hex(8)]
        public int absPtr4;
        [Hex(8)]
        public int absPtr5;
        [Hex(8)]
        public int absPtr6;
        [Hex(8)]
        public int absPtr7;
        [Hex(8)]
        public int absPtr8;
        [Hex(8)]
        public int absPtr9;

        //
        public TopologyParam[] params1 = new TopologyParam[0];
        public TopologyParam[] params2 = new TopologyParam[0];
        public TopologyParam[] params3 = new TopologyParam[0];
        public TopologyParam[] params4 = new TopologyParam[0];
        public TopologyParam[] params5 = new TopologyParam[0];
        public TopologyParam[] params6 = new TopologyParam[0];
        public TopologyParam[] params7 = new TopologyParam[0];
        public TopologyParam[] params8 = new TopologyParam[0];
        public TopologyParam[] params9 = new TopologyParam[0];

        public TopologyParam[][] Params()
        {
            var topology = new TopologyParam[][]
            {
                params1, params2, params3,
                params4, params5, params6,
                params7, params8, params9,
            };

            return topology;
        }

        public int[] Counts()
        {
            return new int[]
            {
                count1, count2, count3,
                count4, count5, count6,
                count7, count8, count9,
            };
        }

        public int[] AbsPtrs()
        {
            return new int[]
            {
                absPtr1, absPtr2, absPtr3,
                absPtr4, absPtr5, absPtr6,
                absPtr7, absPtr8, absPtr9,
            };
        }

        //public Vector3[] Scale()
        //{
        //    var value = new Vector3[];
        //}

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
            startAddress = reader.BaseStream.Position;

            //counts = new int[9];
            //absPtrs = new int[9];

            //reader.ReadX(ref counts, 9);
            //reader.ReadX(ref absPtrs, 9);

            reader.ReadX(ref count1);
            reader.ReadX(ref count2);
            reader.ReadX(ref count3);
            reader.ReadX(ref count4);
            reader.ReadX(ref count5);
            reader.ReadX(ref count6);
            reader.ReadX(ref count7);
            reader.ReadX(ref count8);
            reader.ReadX(ref count9);
            reader.ReadX(ref absPtr1);
            reader.ReadX(ref absPtr2);
            reader.ReadX(ref absPtr3);
            reader.ReadX(ref absPtr4);
            reader.ReadX(ref absPtr5);
            reader.ReadX(ref absPtr6);
            reader.ReadX(ref absPtr7);
            reader.ReadX(ref absPtr8);
            reader.ReadX(ref absPtr9);

            endAddress = reader.BaseStream.Position;

            //var @params = Params();
            //var absPtrs = AbsPtrs();
            //var counts = Counts();
            //for (int i = 0; i < 9; i++)
            //{
            //    reader.BaseStream.Seek(absPtrs[i], SeekOrigin.Begin);
            //    reader.ReadX(ref @params[i], counts[i], true);
            //}


            // 1
            reader.BaseStream.Seek(absPtr1, SeekOrigin.Begin);
            reader.ReadX(ref params1, count1, true);
            // 2
            reader.BaseStream.Seek(absPtr2, SeekOrigin.Begin);
            reader.ReadX(ref params2, count2, true);
            // 3
            reader.BaseStream.Seek(absPtr3, SeekOrigin.Begin);
            reader.ReadX(ref params3, count3, true);
            // 4
            reader.BaseStream.Seek(absPtr4, SeekOrigin.Begin);
            reader.ReadX(ref params4, count4, true);
            // 5
            reader.BaseStream.Seek(absPtr5, SeekOrigin.Begin);
            reader.ReadX(ref params5, count5, true);
            // 6
            reader.BaseStream.Seek(absPtr6, SeekOrigin.Begin);
            reader.ReadX(ref params6, count6, true);
            // 7
            reader.BaseStream.Seek(absPtr7, SeekOrigin.Begin);
            reader.ReadX(ref params7, count7, true);
            // 8
            reader.BaseStream.Seek(absPtr8, SeekOrigin.Begin);
            reader.ReadX(ref params8, count8, true);
            // 9
            reader.BaseStream.Seek(absPtr9, SeekOrigin.Begin);
            reader.ReadX(ref params9, count9, true);

            reader.BaseStream.Seek(endAddress, SeekOrigin.Begin);
        }

        public void Serialize(BinaryWriter writer)
        {
            //writer.WriteX(count1);
            //writer.WriteX(count2);
            //writer.WriteX(count3);
            //writer.WriteX(count4);
            //writer.WriteX(count5);
            //writer.WriteX(count6);
            //writer.WriteX(count7);
            //writer.WriteX(count8);
            //writer.WriteX(count9);
            //writer.WriteX(absPtr1);
            //writer.WriteX(absPtr2);
            //writer.WriteX(absPtr3);
            //writer.WriteX(absPtr4);
            //writer.WriteX(absPtr5);
            //writer.WriteX(absPtr6);
            //writer.WriteX(absPtr7);
            //writer.WriteX(absPtr8);
            //writer.WriteX(absPtr9);

            throw new NotImplementedException();
        }

        #endregion

    }
}
