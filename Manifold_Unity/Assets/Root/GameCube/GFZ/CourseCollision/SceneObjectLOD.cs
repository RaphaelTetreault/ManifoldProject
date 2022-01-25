using Manifold.IO;
using System;
using System.IO;

namespace GameCube.GFZ.CourseCollision
{
    /// <summary>
    /// Binds an object name to a loadable display model.
    /// </summary>
    [Serializable]
    public class SceneObjectLOD :
        IBinaryAddressable,
        IBinarySerializable,
        IHasReference
    {
        // METADATA
        [UnityEngine.SerializeField] private AddressRange addressRange;

        // FIELDS
        public uint zero_0x00;
        public Pointer lodNamePtr;
        public uint zero_0x08;
        public float lodDistance;
        // REFERENCE FIELDS
        public CString name;


        // PROPERTIES
        public AddressRange AddressRange
        {
            get => addressRange;
            set => addressRange = value;
        }


        // METHODS
        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref zero_0x00);
                reader.ReadX(ref lodNamePtr);
                reader.ReadX(ref zero_0x08);
                reader.ReadX(ref lodDistance);
            }
            this.RecordEndAddress(reader);
            {
                Assert.IsTrue(zero_0x00 == 0);
                Assert.IsTrue(zero_0x08 == 0);

                reader.JumpToAddress(lodNamePtr);
                reader.ReadX(ref name, true);
            }
            this.SetReaderToEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            {
                Assert.IsTrue(zero_0x00 == 0);
                Assert.IsTrue(zero_0x08 == 0);

                lodNamePtr = name.GetPointer();
            }
            this.RecordStartAddress(writer);
            {
                writer.WriteX(zero_0x00);
                writer.WriteX(lodNamePtr);
                writer.WriteX(zero_0x08);
                writer.WriteX(lodDistance);
            }
            this.RecordEndAddress(writer);
        }

        public void ValidateReferences()
        {
            // This pointer CANNOT be null and must refer to an object name.

            Assert.IsTrue(name != null); // true null only, name can be string.Empty
            Assert.ReferencePointer(name, lodNamePtr); // 2022/01/25: must always have instance, ptr!

            // Constants
            Assert.IsTrue(zero_0x00 == 0);
            Assert.IsTrue(zero_0x08 == 0);
        }

        public override string ToString()
        {
            return
                $"{nameof(SceneObjectLOD)}(" +
                $"{nameof(lodDistance)}: {lodDistance}, " +
                $"{nameof(name)}: {name}" +
                $")";
        }
    }
}