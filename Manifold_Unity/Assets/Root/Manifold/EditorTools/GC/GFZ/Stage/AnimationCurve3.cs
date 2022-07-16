using Manifold.IO;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage
{
    /// <summary>
    /// Three UnityEngine.AnimationCurves combined to represent X, Y, and Z coordinates.
    /// </summary>
    [System.Serializable]
    public class AnimationCurve3 :
        IDeepCopyable<AnimationCurve3>
    {
        // FIELDS
        [SerializeField] public AnimationCurve x = new AnimationCurve();
        [SerializeField] public AnimationCurve y = new AnimationCurve();
        [SerializeField] public AnimationCurve z = new AnimationCurve();

        public AnimationCurve[] CurvesXYZ
        {
            get => new AnimationCurve[] { x, y, z };
        }

        // METHODS
        public AnimationCurve3 CreateDeepCopy()
        {
            var copy = new AnimationCurve3();
            copy.x = x.GetCopy();
            copy.y = y.GetCopy();
            copy.z = z.GetCopy();
            return copy;
        }

        public Vector3 Evaluate(float time)
        {
            var px = x.Evaluate(time);
            var py = y.Evaluate(time);
            var pz = z.Evaluate(time);
            var point = new Vector3(px, py, pz);

            return point;
        }

        public Vector3 EvaluateDefault(float time, Vector3 @default)
        {
            var px = x.EvaluateDefault(time, @default.x);
            var py = y.EvaluateDefault(time, @default.y);
            var pz = z.EvaluateDefault(time, @default.z);
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
            return new AnimationCurve[] { x, y, z };
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

        public void AddKeys(float time, Vector3 value)
        {
            x.AddKey(time, value.x);
            y.AddKey(time, value.y);
            z.AddKey(time, value.z);
        }

    }

}
