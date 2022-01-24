using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using GameCube.GFZ.CourseCollision;

namespace Manifold.IO.GFZ.CourseCollision
{
    public class GfzLegacyData : MonoBehaviour
    {
        [SerializeField, HideInInspector]
        private ColiScene scene;

        public ColiScene Scene => scene;
    }
}
