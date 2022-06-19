using GameCube.GFZ.Stage;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage
{
    public abstract class GfzTrackEmbededProperty : MonoBehaviour
    {
        // property type?

        public abstract EmbeddedTrackPropertyArea GetEmbededProperty();

        public abstract Mesh GenerateMesh();
    }
}