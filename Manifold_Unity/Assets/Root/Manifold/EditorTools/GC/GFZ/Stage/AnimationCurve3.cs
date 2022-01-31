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

        public float GetMaxTime()
        {
            var xMaxTime = x.GetMaxTime();
            var yMaxTime = y.GetMaxTime();
            var zMaxTime = z.GetMaxTime();

            bool isValid =
                xMaxTime == yMaxTime &&
                yMaxTime == zMaxTime;

            Assert.IsTrue(isValid, "Max times for each curve do not match!");

            return xMaxTime;
        }

    }

}
