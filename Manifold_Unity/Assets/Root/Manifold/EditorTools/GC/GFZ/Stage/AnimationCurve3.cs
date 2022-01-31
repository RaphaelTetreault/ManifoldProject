using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
        public Vector3 Evaluate(float time)
        {
            var px = x.Evaluate(time);
            var py = y.Evaluate(time);
            var pz = z.Evaluate(time);
            var point = new Vector3(px, py, pz);

            return point;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector3 Evaluate(double time)
        {
            return Evaluate((float)time);
        }

        public AnimationCurve[] GetCurves()
        {
            return new AnimationCurve[] {x, y, z };
        }
    }

}
