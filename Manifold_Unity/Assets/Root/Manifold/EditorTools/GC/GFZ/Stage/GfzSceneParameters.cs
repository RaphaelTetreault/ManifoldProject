using GameCube.GFZ.REL;
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
        public GameCube.GFZ.REL.Cup cup = Cup.Ruby;
        public GameCube.GFZ.REL.Venue _venue = GameCube.GFZ.REL.Venue.MuteCity;
        public string courseName;
        [Range(0, 111)]
        public byte courseIndex;
        [Range(1, 10)]
        public byte difficulty = 5;

        [Header("Other Details")]
        [Tooltip("Legacy venue, still used for fog, will be removed eventually.")]
        public GameCube.GFZ.Stage.Venue venue; // TODO: change to VenueName, make indexer helper
        //public string courseName;
        //public int courseIndex; // TODO: validate export venue to index
        public CircuitType circuitType = CircuitType.ClosedCircuit;
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
