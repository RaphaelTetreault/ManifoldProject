using Manifold;
using Manifold.IO;
using System.IO;
using System.Collections.Generic;

namespace GameCube.GFZ.Stage
{
    /// <summary>
    /// A list of indexes (u16). Used to index collider triangles/quads and
    /// track checkpoint nodes.
    /// </summary>
    [System.Serializable]
    public sealed class IndexList :
        IBinaryAddressable,
        IBinarySerializable,
        ITextPrintable
    {
        // CONSTANTS
        public const ushort kUshortArrayTerminator = 0xFFFF;

        // FIELDS
        private ushort[] indexes = new ushort[0];


        // INDEXERS
        public ushort this[int index] { get => indexes[index]; set => indexes[index] = value; }

        // PROPERTIES
        public AddressRange AddressRange { get; set; }
        public ushort[] Indexes { get => indexes; set => indexes = value; }
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
                    AddressRange = new AddressRange();
                }
            }
        }

        public static IndexList CreateIndexList(List<int> indexes)
        {
            var indexList = new IndexList();

            // If we have no indexes, return a base structure. It would be
            // an error to return an array of just [1] with 0xFFFF
            if (indexes.Count == 0)
                return indexList;

            // Copy indexes over, checking to see if there's an overflow
            var _indexes = new ushort[indexes.Count + 1];
            for (int i = 0; i < indexes.Count; i++)
            {
                if (i > ushort.MaxValue - 1)
                    throw new System.Exception("Tried to make an index too large!");

                _indexes[i] = (ushort)indexes[i];
            }

            // The last element is a teminating ushort
            _indexes[indexes.Count] = kUshortArrayTerminator;
            indexList.indexes = _indexes;

            return indexList;
        }

        public void PrintMultiLine(System.Text.StringBuilder builder, int indentLevel = 0, string indent = "\t")
        {
            const int valuesPerRow = 32;
            int numRows = Length / valuesPerRow;
            int numPadding = Length.ToString().Length;

            // Write a little helper heading
            builder.AppendIndented(indent, indentLevel, "");
            builder.AppendRepeat(" ", numPadding + 3); // +3 = "[] "
            for (int i = 0; i < valuesPerRow; i++)
            {
                builder.Append($"{i+1,4}, ");
            }

            // Write all values, 32 per line
            for (int r = 0; r < numRows; r++)
            {
                builder.AppendIndented(indent, indentLevel, $"[{Length.ToString().PadLeft(numPadding)}] ");
                for (int i = 0; i < valuesPerRow; i++)
                {
                    var index = r * valuesPerRow + i;
                    var value = indexes[index];
                    builder.Append($"{value:x4}, ");
                }
                builder.AppendLine();
            }
        }

        public string PrintSingleLine()
        {
            return $"{nameof(IndexList)}[{indexes.Length}]";
        }

        public override string ToString() => PrintSingleLine();

    }
}
