using GameCube.GFZ.CourseCollision;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage
{
    public abstract class GfzTrackEmbededProperty : MonoBehaviour
    {
        // property type?

        public abstract SurfaceAttributeArea GetEmbededProperty();

        public abstract Mesh GenerateMesh();
    }
}
