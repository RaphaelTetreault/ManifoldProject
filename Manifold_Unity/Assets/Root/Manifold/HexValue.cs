using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hex : LabelPrefix
{
    [SerializeField] string format = "X";
    [SerializeField] int width = 0;

    public string Format => format;
    public int Width => width;

    public Hex(string prefix = null, string format = "X", int w = 0)
        : base (prefix)
    {
        this.format = format;
        this.width = w;
    }
}
