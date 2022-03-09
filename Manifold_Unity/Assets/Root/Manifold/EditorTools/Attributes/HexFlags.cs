using UnityEngine;

namespace Manifold.EditorTools.Attributes
{
    public class HexFlags : LabelPrefix
    {
        public int NumDigits { get; private set; }
        public string Format => $"X{NumDigits}";

        public HexFlags(string prefix = null, int numDigits = 0)
            : base(prefix)
        {
            this.NumDigits = numDigits;
        }
    }
}