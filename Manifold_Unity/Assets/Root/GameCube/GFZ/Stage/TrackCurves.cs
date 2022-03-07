using Manifold;
using Manifold.EditorTools;
using Manifold.EditorTools.GC.GFZ;
using Manifold.IO;
using System;
using System.IO;


namespace GameCube.GFZ.Stage
{
    /// <summary>
    /// Defines the tracks position, rotation, and scale using 9 animation curves,
    /// each defining the X, Y, and Z properties. Their values are (assummed to be)
    /// multiplied with an associated Transform. Hierarchies of these exists, each
    /// being multiplied with it's parent up a tree structure.
    /// </summary>
    [Serializable]
    public class TrackCurves :
        IBinaryAddressable,
        IBinarySerializable,
        IHasReference
    {
        // CONSTANTS
        public const int kCurveCount = 9;


        // FIELDS
        private ArrayPointer2D curvesPtr2D = new ArrayPointer2D(kCurveCount);
        // REFERENCE FIELDS
        private AnimationCurve[] animationCurves = new AnimationCurve[0];


        // INDEXERS
        public AnimationCurve this[int index]
        {
            get => animationCurves[index];
            set => animationCurves[index] = value;
        }

        // PROPERTIES
        public AddressRange AddressRange { get; set; }


        public AnimationCurve ScaleX
        {
            get => AnimationCurves[0];
            set => AnimationCurves[0] = value;
        }
        public AnimationCurve ScaleY
        {
            get => AnimationCurves[1];
            set => AnimationCurves[1] = value;
        }
        public AnimationCurve ScaleZ
        {
            get => AnimationCurves[2];
            set => AnimationCurves[2] = value;
        }

        public AnimationCurve RotationX
        {
            get => AnimationCurves[3];
            set => AnimationCurves[3] = value;
        }
        public AnimationCurve RotationY
        {
            get => AnimationCurves[4];
            set => AnimationCurves[4] = value;
        }
        public AnimationCurve RotationZ
        {
            get => AnimationCurves[5];
            set => AnimationCurves[5] = value;
        }

        public AnimationCurve PositionX
        {
            get => AnimationCurves[6];
            set => AnimationCurves[6] = value;
        }
        public AnimationCurve PositionY
        {
            get => AnimationCurves[7];
            set => AnimationCurves[7] = value;
        }
        public AnimationCurve PositionZ
        {
            get => AnimationCurves[8];
            set => AnimationCurves[8] = value;
        }

        public ArrayPointer2D CurvesPtr2D { get => curvesPtr2D; set => curvesPtr2D = value; }
        public AnimationCurve[] AnimationCurves { get => animationCurves; set => animationCurves = value; }


        // METHODS
        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                curvesPtr2D.Deserialize(reader);
            }
            this.RecordEndAddress(reader);
            {
                // Init array
                AnimationCurves = new AnimationCurve[kCurveCount];

                for (int i = 0; i < AnimationCurves.Length; i++)
                {
                    var arrayPointer = curvesPtr2D.ArrayPointers[i];
                    if (arrayPointer.IsNotNull)
                    {
                        // Deserialization is done to instance with properties set through constructor.
                        reader.JumpToAddress(arrayPointer);
                        var animationCurve = new AnimationCurve(arrayPointer.Length);
                        animationCurve.Deserialize(reader);

                        // Assign curve to array
                        AnimationCurves[i] = animationCurve;
                    }
                    else
                    {
                        AnimationCurves[i] = new AnimationCurve(0);
                    }
                }
            }
            this.SetReaderToEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            {
                // Ensure we have the correct amount of animation curves before indexing
                Assert.IsTrue(AnimationCurves.Length == kCurveCount);

                // Construct ArrayPointer2D for animation curves
                var pointers = new ArrayPointer[kCurveCount];
                for (int i = 0; i < pointers.Length; i++)
                    pointers[i] = AnimationCurves[i].GetArrayPointer();

                curvesPtr2D = new ArrayPointer2D(pointers);
            }
            this.RecordStartAddress(writer);
            {
                writer.WriteX(curvesPtr2D);
            }
            this.RecordEndAddress(writer);
        }

        public void ValidateReferences()
        {
            // Assert array information
            Assert.IsTrue(AnimationCurves != null);
            Assert.IsTrue(AnimationCurves.Length == kCurveCount);
            Assert.IsTrue(CurvesPtr2D.Length == kCurveCount);

            // Assert each animation curve
            for (int i = 0; i < kCurveCount; i++)
            {
                var animCurve = AnimationCurves[i];
                var pointer = CurvesPtr2D.ArrayPointers[i];
                
                // Only assert if there are keyables
                if (animCurve.Length != 0)
                    Assert.ReferencePointer(animCurve, pointer);
            }
        }

    }
}
