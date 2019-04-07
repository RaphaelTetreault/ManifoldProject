using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LabelPrefix : PropertyAttribute
{
    public string prefix;

    public LabelPrefix(string prefix)
    {
        this.prefix = prefix;
    }
}
