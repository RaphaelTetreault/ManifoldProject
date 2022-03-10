using Manifold;
using Manifold.IO;
using System;
using System.IO;

namespace GameCube.GFZ.Stage
{
    /// <summary>
    /// An object which does not use a transform for placement within the scene. The model's
    /// origin will align with the scene's origin.
    /// 
    /// This kind of object was used for all test objects in old AX scenes.
    /// </summary>
    [Serializable]
    public sealed class SceneObjectStatic :
        IBinaryAddressable,
        IBinarySerializable,
        IHasReference,
        ITextPrintable
    {
        // FIELDS
        private Pointer sceneObjectPtr;
        // REFERENCE FIELDS
        private SceneObject sceneObject;


        // PROPERTIES
        public AddressRange AddressRange { get; set; }
        public string Name => SceneObject.Name;

        public SceneObject SceneObject { get => sceneObject; set => sceneObject = value; }
        public Pointer SceneObjectPtr { get => sceneObjectPtr; set => sceneObjectPtr = value; }


        // METHODS
        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref sceneObjectPtr);
            }
            this.RecordEndAddress(reader);
            {
                Assert.IsTrue(sceneObjectPtr.IsNotNull);
                reader.JumpToAddress(sceneObjectPtr);
                reader.ReadX(ref sceneObject);
            }
            this.SetReaderToEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            {
                sceneObjectPtr = sceneObject.GetPointer();
            }
            this.RecordStartAddress(writer);
            {
                writer.WriteX(sceneObjectPtr);
            }
            this.RecordEndAddress(writer);

        }

        public void ValidateReferences()
        {
            Assert.IsTrue(sceneObjectPtr.IsNotNull);
        }

        public void PrintMultiLine(System.Text.StringBuilder builder, int indentLevel = 0, string indent = "\t")
        {
            builder.AppendLineIndented(indent, indentLevel, PrintSingleLine());
        }

        public string PrintSingleLine()
        {
            return $"{nameof(SceneObjectStatic)}({Name})";
        }

        public override string ToString() => PrintSingleLine();
    }
}