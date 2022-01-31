using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage
{
    [System.Serializable]
    public class AnimationCurve3
    {
        [SerializeField] public AnimationCurve x = new AnimationCurve();
        [SerializeField] public AnimationCurve y = new AnimationCurve();
        [SerializeField] public AnimationCurve z = new AnimationCurve();

        // TODO: make a faster version of this? Store max value?
        public Vector3 EvaluateNormalized(float timeNormalized)
        {
            var px = x.EvaluateNormalized(timeNormalized);
            var py = y.EvaluateNormalized(timeNormalized);
            var pz = z.EvaluateNormalized(timeNormalized);
            var point = new Vector3(px, py, pz);

            return point;
        }

        public Vector3 EvaluateNormalized(double timeNormalized)
        {
            return EvaluateNormalized((float)timeNormalized);
        }

        public AnimationCurve[] GetCurves()
        {
            return new AnimationCurve[] {x, y, z };
        }
    }

}
