using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manifold.EditorTools.Attributes
{
    public class LabelPrefix : PropertyAttribute
    {
        public string prefix;

        public LabelPrefix(string prefix)
        {
            this.prefix = prefix;
        }
    }
}