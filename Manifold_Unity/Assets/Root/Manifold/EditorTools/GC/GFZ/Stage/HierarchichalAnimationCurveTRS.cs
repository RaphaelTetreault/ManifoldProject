using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage
{
    [System.Serializable]
    public class HierarchichalAnimationCurveTRS
    {
        [field: SerializeField] public AnimationCurveTRS TRS { get; set; } = new();
        [field: SerializeField] public HierarchichalAnimationCurveTRS Parent { get; set; }
        [field: SerializeField] public HierarchichalAnimationCurveTRS[] Children { get; set; }
    }
}
