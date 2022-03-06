﻿using Manifold;
using Manifold.IO;
using System;
using System.IO;

namespace GameCube.GFZ.CourseCollision
{
    /// <summary>
    /// A series of animation keys to define how a singular float value changes over "time"
    /// 
    /// Structurally, thus class is a simple wrapper around a KeyableAttribute[].
    /// </summary>
    [Serializable]
    public class AnimationCurve :
        IBinaryAddressable,
        IBinarySerializable,
        ITextPrintable
    {
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
        public AnimationCurve(KeyableAttribute[] keyables)
        {
            keyableAttributes = keyables;
        }


        // PROPERTIES
        public AddressRange AddressRange { get; set; }

        public int Length => keyableAttributes.Length;


        // METHODS
        public void Deserialize(BinaryReader reader)
        {
            this.RecordStartAddress(reader);
            {
                reader.ReadX(ref keyableAttributes, keyableAttributes.Length);
            }
            this.RecordEndAddress(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            if (keyableAttributes.Length > 0)
            {
                this.RecordStartAddress(writer);
                {
                    foreach (var keyable in keyableAttributes)
                        writer.WriteX(keyable);
                }
                this.RecordEndAddress(writer);
            }
            else
            {
                // If nothing to serialize, zero out address as
                // it will be used to get ptr
                AddressRange = new AddressRange();
            }
        }

        /// <summary>
        /// Forwards the underlying KeyableAttributes[].GetArrayPointer();
        /// </summary>
        /// <returns></returns>
        public ArrayPointer GetArrayPointer()
        {
            return keyableAttributes.GetArrayPointer();
        }

        public override string ToString()
        {
            return PrintSingleLine();
        }

        public string PrintSingleLine()
        {
            // Print a summary of how many animation keys this curve has
            return $"{nameof(AnimationCurve)}({nameof(KeyableAttribute)}s:[{keyableAttributes.Length}])";
        }

        public string PrintMultiLine(int indentLevel = 0, string indent = "\t")
        {
            int lengthKeyables = keyableAttributes.Length;
            var stringBuilder = new System.Text.StringBuilder();

            // Print overview of the animation curve
            stringBuilder.AppendLineIndented(indent, indentLevel, PrintSingleLine());
            indentLevel++;
            // Then print each keyable (single line) on their own line
            for (int i = 0; i < lengthKeyables; i++)
            {
                var keyable = keyableAttributes[i];
                var keyableText = keyable.PrintSingleLine();
                stringBuilder.AppendLineIndented(indent, indentLevel, $"[{i}]\t {keyableText}");
            }

            return stringBuilder.ToString();
        }
    }
}
