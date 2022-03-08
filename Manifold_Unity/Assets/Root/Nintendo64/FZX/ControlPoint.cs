using System;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using Manifold.IO;

namespace Nintendo64.FZX
{
    [Serializable]
    public class ControlPoint
    {
        public float x;
        public float y;
        public float z;
        public float widthL;
        public float widthR;
        public ushort banking; // fixedpoint?
        public TrackType trackType;
       
        public Vector3 Position => new Vector3(x, y, z);
        public float FullWidth => widthL + widthR;

    }

}
