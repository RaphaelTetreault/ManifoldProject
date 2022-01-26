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

        public Vector3 Evaluate(float time)
        {
            var point = new Vector3()
            {
                x = x.Evaluate(time),
                y = y.Evaluate(time),
                z = z.Evaluate(time),
            };

            return point;
        }
    }
}
