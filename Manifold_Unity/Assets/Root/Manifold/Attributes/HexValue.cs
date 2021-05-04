using System;

public class Hex : LabelPrefix
{
    public Type type;
    public int numDigits { get; private set; }
    public string Format => $"{format}{numDigits}";
    public string format { get; private set; }

    public int Width { get; private set; }

    public Hex(string prefix = null, int numDigits = 0, Type type = null, string format = "X")
        : base (prefix)
    {
        this.numDigits = numDigits;
        this.type = type;
        this.format = format;
    }
    public Hex(int numDigits) : this(null, numDigits, null) { }
    public Hex(string prefix) : this(prefix, 0, null) { }
}
