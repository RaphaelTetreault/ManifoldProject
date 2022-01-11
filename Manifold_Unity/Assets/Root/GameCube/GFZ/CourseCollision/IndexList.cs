using Manifold.IO;
using System.IO;
using System.Collections.Generic;

namespace GameCube.GFZ.CourseCollision
{
    /// <summary>
    /// A list of indexes (u16). Used to index collider triangles/quads and
    /// track checkpoint nodes.
    /// </summary>
    [System.Serializable]
    public class IndexList :
        IBinaryAddressable,
        IBinarySerializable
    {
        // CONSTANTS
        public const ushort kUshortArrayTerminator = 0xFFFF;

        // METADATA
        [UnityEngine.SerializeField] private AddressRange addressRange;

        // FIELDS
        [UnityEngine.SerializeField] private ushort[] indexes = new ushort[0];


        // PROPERTIES
        public AddressRange AddressRange
        {
            get => addressRange;
            set => addressRange = value;
        }

        public ushort[] Indexes
        {
            get => indexes;
            set => indexes = value;
        }

        public int Length => indexes.Length;


        // METHODS
        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                var list = new List<ushort>();
                while (true)
                {
                    // Read next value
                    var value = reader.ReadX_UInt16();
                    // Break loop, don't add value if terminator
                    if (value == kUshortArrayTerminator)
                    {
                        break;
                    }
                    // Add value to collection
                    list.Add(value);
                }
                indexes = list.ToArray();
            }
            this.RecordEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            this.RecordStartAddress(writer);
            {
                // Only serialize list if we have indexes to serialize.
                // Otherwise we accidentally serialize a null terminator.
                if (Length > 0)
                {
                    this.RecordStartAddress(writer);
                    {
                        // Write each index
                        foreach (var index in indexes)
                        {
                            writer.WriteX(index);
                        }
                        // Write terminating character
                        writer.WriteX(kUshortArrayTerminator);
                    }
                    this.RecordEndAddress(writer);
                }
                else
                {
                    // Reset AddressRange as it WILL be converted
                    // to pointer. Since we are "null", reset = null ptr.
                    addressRange = new AddressRange();
                }
            }
        }
    }
}
