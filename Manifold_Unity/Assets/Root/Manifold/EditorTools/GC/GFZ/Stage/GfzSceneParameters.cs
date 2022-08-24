using GameCube.GFZ.Stage;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage
{
    /// <summary>
    /// Component class which stores general information regarding the scene.
    /// </summary>
    public class GfzSceneParameters : MonoBehaviour
        //IGfzConvertable<Fog>,
        //IGfzConvertable<FogCurves>
    {
        [Header("About")]
        public string author;

        [Header("Course Details")]
        public Venue venue; // TODO: change to VenueName, make indexer helper
        public string courseName;
        public int courseIndex; // TODO: validate export venue to index
        public CircuitType circuitType = CircuitType.ClosedCircuit; // will become param from control points.
        public Bool32 staticColliderMeshesActive = Bool32.True;

        [Header("Unknown Range")]
        public float rangeNear;
        public float rangeFar;

        public string GetGfzInternalName()
        {
            return $"COLI_COURSE{courseIndex:00}";
        }

        public string GetGfzDisplayName()
        {
            var venueName = EnumExtensions.GetDescription(venue);
            return $"{venueName} [{courseName}]";
        }

    }
}
