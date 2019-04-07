using System;

public class Hex : LabelPrefix
{
    public Type type;
    public int numDigits { get; private set; }
    public string Format => $"X{numDigits}";

    public int Width { get; private set; }

    public Hex(string prefix = null, int numDigits = 0, Type type = null)
        : base (prefix)
    {
        this.numDigits = numDigits;
        this.type = type;
    }
    public Hex(int numDigits) : this(null, numDigits, null) { }
    public Hex(string prefix) : this(prefix, 0, null) { }
}
