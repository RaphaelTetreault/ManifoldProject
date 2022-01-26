using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manifold
{
    [System.Serializable]
    public class AnimationCurve3
    {
        [SerializeField] public AnimationCurve x = new AnimationCurve();
        [SerializeField] public AnimationCurve y = new AnimationCurve();
        [SerializeField] public AnimationCurve z = new AnimationCurve();


        //public Vector3 Evaluate(float time)
        //{
        //    var point = new Vector3()
        //    {
        //        x = x.Evaluate(time),
        //        y = y.Evaluate(time),
        //        z = z.Evaluate(time),
        //    };

        //    return point;
        //}

        // TODO: make a faster version of this? Store max value?
        public Vector3 EvaluateNormalized(float time)
        {
            var point = new Vector3()
            {
                x = x.EvaluateNormalized(time),
                y = y.EvaluateNormalized(time),
                z = z.EvaluateNormalized(time),
            };

            return point;
        }

        //public void SanityCheck()
        //{
        //    var curves = new AnimationCurve[] { x, y, z };
        //    var maxTime = new float[curves.Length];

        //    for (int i = 0; i < curves.Length; i++)
        //    {
        //        var curve = curves[i];
        //        if (curve.keys.Length == 0)
        //        {
        //            maxTime[i] = 0f;
        //        }
        //        else // we have 1 or more keys
        //        {
        //            var index = curve.keys.Length - 1;
        //            maxTime[i] = (curve.keys[index].time);
        //        }
        //    }

        //    var allIsSame = maxTime[0] == maxTime[1] && maxTime[1] == maxTime[2];
        //    Assert.IsTrue(allIsSame);
        //}

    }
}
