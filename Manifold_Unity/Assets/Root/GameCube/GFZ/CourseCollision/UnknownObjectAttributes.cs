using Manifold.IO;
using System;
using System.IO;
using System.Text;

namespace GameCube.GFZ.CourseCollision
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class UnknownObjectAttributes :
        IBinarySeralizableReference
    {
        // METADATA
        [UnityEngine.SerializeField]
        private AddressRange addressRange;
        public string name;

        // FIELDS
        public uint unk_0x00;
        public Pointer namePtr;
        public uint unk_0x08;
        public float unk_0x0C;


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
                reader.ReadX(ref unk_0x00);
                reader.ReadX(ref namePtr);
                reader.ReadX(ref unk_0x08);
                reader.ReadX(ref unk_0x0C);
            }
            this.RecordEndAddress(reader);
            {
                reader.JumpToAddress(namePtr);
                reader.ReadXCString(ref name, Encoding.ASCII);
            }
            this.SetReaderToEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            this.RecordStartAddress(writer);
            {
                writer.WriteX(unk_0x00);

                // Add this string to a table to be serialized at the end of serializing
                // a ColiScene file.
                ColiCourseUtility.stringTable.Add(
                    new ColiCourseUtility.StringTableEntry()
                    {
                        // This address to com back to and overwrite with the string pointer
                        pointer = writer.GetPositionAsPointer(),
                        // what we want to serialize: the name
                        value = name,
                        // add some metadata to help sort. Will be printed if SerializeVerbose
                        type = ColiCourseUtility.StringTableEntry.Type.unknownObjectsAttributes,
                    });
                // write whatever for now
                writer.WriteX(namePtr);

                writer.WriteX(unk_0x08);
                writer.WriteX(unk_0x0C);
            }
            this.RecordEndAddress(writer);
        }

        public AddressRange SerializeWithReference(BinaryWriter writer)
        {
            Serialize(writer);
            return addressRange;
        }
    }
}