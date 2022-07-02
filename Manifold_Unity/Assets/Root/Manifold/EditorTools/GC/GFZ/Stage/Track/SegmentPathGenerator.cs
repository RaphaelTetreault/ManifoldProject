using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameCube.GFZ.Stage;

namespace Manifold.EditorTools.GC.GFZ.Stage
{
    public abstract class SegmentPathGenerator : MonoBehaviour
    {
        public event System.Action OnEdited;

        public abstract AnimationCurveTRS GetAnimationCurveTRS();

        protected void CallOnEdited()
        {
            OnEdited?.Invoke();
        }
    }
}
