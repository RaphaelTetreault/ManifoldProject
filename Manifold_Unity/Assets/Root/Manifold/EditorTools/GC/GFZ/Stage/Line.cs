using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage
{
    

    public class Line : MonoBehaviour
    {
        [Serializable]
        public struct Point
        {
            public Point(Vector3 position)
            {
                this.position = position;
            }

            public Vector3 position;
        }

        public Point p0;
        public Point p1;
    }
}
