using UnityEngine;

public class EnumFlags : PropertyAttribute
{
    public EnumFlags() { }
}

public class HexEnumFlags : LabelPrefix
{
    public string format;
    public bool prepend0x;

    public HexEnumFlags(string prefix, string format = "X", bool prepend0x = true)
        : base(prefix)
    {
        this.format = format;
        this.prepend0x = prepend0x;
    }
}
