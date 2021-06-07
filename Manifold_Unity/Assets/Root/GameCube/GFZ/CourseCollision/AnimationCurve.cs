using Manifold.IO;
using System;
using System.IO;

namespace GameCube.GFZ.CourseCollision
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class AnimationCurve :
        IBinaryAddressable,
        IBinarySerializable
    {
        // METADATA
        [UnityEngine.SerializeField] private AddressRange addressRange;

        // FIELDS
        public KeyableAttribute[] keyableAttributes;


        // CONSTRUCTORS
        public AnimationCurve()
        {
            keyableAttributes = new KeyableAttribute[0];
        }
        public AnimationCurve(int numKeyables)
        {
            keyableAttributes = new KeyableAttribute[numKeyables];
        }


        // PROPERTIES
        public AddressRange AddressRange
        {
            get => addressRange;
            set => addressRange = value;
        }

        public int Length => keyableAttributes.Length;


        // METHODS
        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref keyableAttributes, keyableAttributes.Length, true);
            }
            this.RecordEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            this.RecordStartAddress(writer);
            {
                foreach (var keyable in keyableAttributes)
                    writer.WriteX(keyable);
            }
            this.RecordEndAddress(writer);
        }

        /// <summary>
        /// Forwards the underlying KeyableAttributes[].GetArrayPointer();
        /// </summary>
        /// <returns></returns>
        public ArrayPointer GetArrayPointer()
        {
            return keyableAttributes.GetArrayPointer();
        }

    }
}
