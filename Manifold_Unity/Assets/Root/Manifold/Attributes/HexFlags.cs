using UnityEngine;

public class HexFlags : LabelPrefix
{
    public int NumDigits { get; private set; }
    public string Format => $"X{NumDigits}";
    public bool Prepend0x => false;// { get; private set; }

    public HexFlags(string prefix, int numDigits = 0, bool prepend0x = true)
        : base(prefix)
    {
        this.NumDigits = numDigits;
        //this.Prepend0x = prepend0x;
    }
}
