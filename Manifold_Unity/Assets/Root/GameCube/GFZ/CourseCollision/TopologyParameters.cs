using Manifold.IO;
using System;
using System.IO;
using UnityEngine;

// TODO: replace counts/absptr with ArrayPointers? Make new [][]Pointer struct?

namespace GameCube.GFZ.CourseCollision
{
    [Serializable]
    public class TopologyParameters : IBinarySerializable, IBinaryAddressableRange
    {

        public const int kFieldCount = 9;

        [SerializeField]
        private AddressRange addressRange;


        public int count1;
        public int count2;
        public int count3;
        public int count4;
        public int count5;
        public int count6;
        public int count7;
        public int count8;
        public int count9;
        public int absPtr1;
        public int absPtr2;
        public int absPtr3;
        public int absPtr4;
        public int absPtr5;
        public int absPtr6;
        public int absPtr7;
        public int absPtr8;
        public int absPtr9;

        //
        public KeyableAttribute[] params1 = new KeyableAttribute[0];
        public KeyableAttribute[] params2 = new KeyableAttribute[0];
        public KeyableAttribute[] params3 = new KeyableAttribute[0];
        public KeyableAttribute[] params4 = new KeyableAttribute[0];
        public KeyableAttribute[] params5 = new KeyableAttribute[0];
        public KeyableAttribute[] params6 = new KeyableAttribute[0];
        public KeyableAttribute[] params7 = new KeyableAttribute[0];
        public KeyableAttribute[] params8 = new KeyableAttribute[0];
        public KeyableAttribute[] params9 = new KeyableAttribute[0];

        public UnityEngine.AnimationCurve curve1;

        public KeyableAttribute[][] Params()
        {
            var topology = new KeyableAttribute[][]
            {
                params1, params2, params3,
                params4, params5, params6,
                params7, params8, params9,
            };

            return topology;
        }

        public AddressRange AddressRange
        {
            get => addressRange;
            set => addressRange = value;
        }


        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
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
            }
            this.RecordEndAddress(reader);
            {
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
            }
            //
            {
                var keyframes = new Keyframe[params1.Length];
                for (int i = 0; i < keyframes.Length; i++)
                {
                    var key = params1[i];
                    var keyframe = new Keyframe(key.time, key.value, key.zTangentIn, key.zTangentOut);
                    keyframes[i] = keyframe;
                }
                curve1 = new UnityEngine.AnimationCurve(keyframes);

                // Transfer keyframe
                for (int i = 0; i < params1.Length; i++)
                {
                    var key = params1[i];

                    UnityEditor.AnimationUtility.TangentMode mode;
                    switch (key.easeMode)
                    {
                        case InterpolationMode.Constant:
                            mode = UnityEditor.AnimationUtility.TangentMode.Constant;
                            break;

                        case InterpolationMode.Linear:
                            mode = UnityEditor.AnimationUtility.TangentMode.Linear;
                            break;

                        case InterpolationMode.unknown1:
                            mode = UnityEditor.AnimationUtility.TangentMode.Free;
                            break;

                        case InterpolationMode.unknown2:
                            mode = UnityEditor.AnimationUtility.TangentMode.;
                            break;

                        default:
                            throw new NotImplementedException();
                    }
                    UnityEditor.AnimationUtility.SetKeyLeftTangentMode(curve1, i, mode);
                    UnityEditor.AnimationUtility.SetKeyRightTangentMode(curve1, i, mode);
                }
                // end
            }
            this.SetReaderToEndAddress(reader);
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

    }
}
