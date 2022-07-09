using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameCube.GFZ.Stage;

namespace Manifold.EditorTools.GC.GFZ.Stage.Track
{
    public abstract class SegmentPathGenerator : MonoBehaviour
    {
        public event System.Action OnEdited;

        public abstract AnimationCurveTRS GenerateAnimationCurveTRS();

        protected void CallOnEdited()
        {
            OnEdited?.Invoke();
        }
    }
}
