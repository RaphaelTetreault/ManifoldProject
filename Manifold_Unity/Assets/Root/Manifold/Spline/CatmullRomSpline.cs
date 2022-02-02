using Unity.Mathematics;

namespace Manifold
{
    public class CatmullRomSpline :
        ISpline
    {
        public const double UniformAlpha = 0f;
        public const double CentripetalAlpha = 0.5f;
        public const double ChordalAlpha = 1f;

        /// <summary>
        /// Get the time value appropriate for a Catmull-Rom spline
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="alpha"></param>
        /// <returns></returns>
        public static double GetTime(double3 from, double3 to, double alpha)
        {
            // Get the difference between the two points
            double3 delta = to - from;

            // delta to the power of two, each componenet summed
            double squaredSum =
                delta.x * delta.x +
                delta.y * delta.y +
                delta.z * delta.z;

            // 
            double power = alpha * 0.5f;
            double time = math.pow(squaredSum, power);

            return time;
        }

        /// <summary>
        /// Perform a single step of Barry and Goldman's pyramidal formulation
        /// </summary>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <param name="t0"></param>
        /// <param name="t1"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static double3 BarryGoldmanPyramidalFormulationStep(double3 p0, double3 p1, double t0, double t1, double t)
        {
            // Single iteration of Barry and Goldman's pyramidal formulation
            // https://en.wikipedia.org/wiki/File:Cubic_Catmull-Rom_formulation.png

            var value =
                (t1 - t) / (t1 - t0) * p0 +
                (t - t0) / (t1 - t0) * p1;
            
            return value;
        }

        /// <summary>
        /// Perform Barry and Goldman's pyramidal formulation for the provided points
        /// </summary>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        /// <param name="t0"></param>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <param name="t3"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static double3 BarryGoldmanPyramidalFormulation(double3 p0, double3 p1, double3 p2, double3 p3, double t0, double t1, double t2, double t3, double t)
        {
            // Barry and Goldman's pyramidal formulation
            // https://en.wikipedia.org/wiki/File:Cubic_Catmull-Rom_formulation.png

            // Formulation between 4 points provided
            double3 a1 = BarryGoldmanPyramidalFormulationStep(p0, p1, t0, t1, t);
            double3 a2 = BarryGoldmanPyramidalFormulationStep(p1, p2, t1, t2, t);
            double3 a3 = BarryGoldmanPyramidalFormulationStep(p2, p3, t2, t3, t);
            // Formulation between above 3 intervals
            double3 b1 = BarryGoldmanPyramidalFormulationStep(a1, a2, t0, t2, t);
            double3 b2 = BarryGoldmanPyramidalFormulationStep(a2, a3, t1, t3, t);
            // Formulation between above 2 intervals
            double3 c = BarryGoldmanPyramidalFormulationStep(b1, b2, t1, t2, t);

            return c;
        }

        /// <summary>
        /// Sample the Catmull-Rom spline at time <paramref name="t"/>
        /// </summary>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        /// <param name="alpha"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static double3 Evaluate(double3 p0, double3 p1, double3 p2, double3 p3, double alpha, double t)
        {
            // Get times for these points (unpacking tuple)
            (double t0, double t1, double t2, double t3) = GetPointTime(p0, p1, p2, p3, alpha);

            double catmullRomTime = math.lerp(t1, t2, t);

            double3 point = BarryGoldmanPyramidalFormulation(p0, p1, p2, p3, t0, t1, t2, t3, catmullRomTime);
            return point;
        }


        /// <summary>
        /// Sample the Catmull-Rom spline <paramref name="nIterations"/> times
        /// </summary>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        /// <param name="alpha"></param>
        /// <param name="nIterations"></param>
        /// <returns></returns>
        public static double3[] EvaluateMany(double3 p0, double3 p1, double3 p2, double3 p3, double alpha, int nIterations)
        {
            double3[] points = new double3[nIterations];

            // Get times for these points (unpacking tuple)
            (double t0, double t1, double t2, double t3) = GetPointTime(p0, p1, p2, p3, alpha);

            // 
            var timeMax = nIterations - 1;
            for (int i = 0; i < nIterations; i++)
            {
                // Iteration Time
                double it = (double)i / timeMax;
                double catmullRomTime = math.lerp(t1, t2, it);
                points[i] = BarryGoldmanPyramidalFormulation(p0, p1, p2, p3, t0, t1, t2, t3, catmullRomTime);
            }

            return points;
        }


        public static (double t0, double t1, double t2, double t3) GetPointTime(double3 p0, double3 p1, double3 p2, double3 p3, double alpha)
        {
            // Get the times for the 4 points priovided
            double t0 = 0f;
            double t1 = GetTime(p0, p1, alpha);
            double t2 = GetTime(p1, p2, alpha) + t1;
            double t3 = GetTime(p2, p3, alpha) + t2;
            return (t0, t1, t2, t3);
        }




        public static (double3 position, double3 direction) HackGetPpsitionDirection(double3 p0, double3 p1, double3 p2, double3 p3, double alpha, double t)
        {
            // Get times for these points (unpacking tuple)
            (double t0, double t1, double t2, double t3) = GetPointTime(p0, p1, p2, p3, alpha);
            double catmullRomTime = math.lerp(t1, t2, t);
            // Sample point at this time
            double3 point = BarryGoldmanPyramidalFormulation(p0, p1, p2, p3, t0, t1, t2, t3, catmullRomTime);

            // This is the hack piece: sample curve very slightly ahead, get delta
            double3 pointNext = BarryGoldmanPyramidalFormulation(p0, p1, p2, p3, t0, t1, t2, t3, catmullRomTime + 0.00001);
            double3 direction = pointNext - point;

            return (point, direction);
        }

    }
}