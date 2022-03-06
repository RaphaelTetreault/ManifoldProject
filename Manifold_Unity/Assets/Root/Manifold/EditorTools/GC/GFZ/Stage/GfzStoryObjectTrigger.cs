using GameCube.GFZ.Stage;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage
{
    public class GfzStoryObjectTrigger : MonoBehaviour,
        IGfzConvertable<StoryObjectTrigger>
    {
        // INSPECTOR FIELDS
        [SerializeField] private byte order;
        [SerializeField] private byte group;
        [SerializeField] private StoryDifficulty difficulty;

        // PROPERTIES
        public byte Order
        {
            get => order;
            set => order = value;
        }
        public byte Group
        {
            get => group;
            set => group = value;
        }
        public StoryDifficulty Difficulty
        {
            get => difficulty;
            set => difficulty = value;
        
        }

        // METHODS
        public StoryObjectTrigger ExportGfz()
        {
            throw new System.NotImplementedException();

            //// TODO: find best way to copy transform over without the GFZ Transform 
            //// Convert unity transform to gfz transform
            ////var transform = TransformConverter.ToGfzTransform(this.transform);

            //var value = new StoryObjectTrigger
            //{

            //};

            //return value;
        }

        public void ImportGfz(StoryObjectTrigger value)
        {
            Debug.LogWarning($"{nameof(GfzStoryObjectTrigger)} did not properly assign values.");
            //throw new System.NotImplementedException();
        }

    }
}
