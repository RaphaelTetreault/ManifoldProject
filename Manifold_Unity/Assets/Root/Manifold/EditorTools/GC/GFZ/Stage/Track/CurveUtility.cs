using System;
using Unity.Mathematics;

namespace Manifold.EditorTools.GC.GFZ.Stage.Track
{
    public static class CurveUtility
    {

        /// <summary>
        /// Continually increases sampling precision until distance gained is less than <paramref name="minDelta"/>
        /// </summary>
        /// <param name="timeStart"></param>
        /// <param name="timeEnd"></param>
        /// <param name="minDelta"></param>
        /// <param name="powerBase"></param>
        /// <param name="powerExp"></param>
        /// <returns></returns>
        public static double GetDistanceBetweenRepeated(IPositionEvaluable evaluable, double timeStart, double timeEnd, float minDelta = 0.01f,  int powerBase = 2, int powerExp = 1)
        {
            const int powerExpMin = 1;
            const int powerExpMax = 20;
            if (powerExp < powerExpMin || powerExp >= powerExpMax)
                throw new ArgumentOutOfRangeException($"{nameof(powerExp)} must be between {powerExpMin} and {powerExpMax-2}");

            // Limit on how many cycles we can do
            const int maxIterations = 1 << powerExpMax; // 2^20 = 1,048,576

            // Values to store distance state between sampling points
            double delta = 0f;
            double currDistance = 0f;
            double prevDistance = 0f;

            do
            {
                // Compute how many samplings to do in this loop iteration
                int iterations = (int)math.pow(powerBase, powerExp);
                powerExp++;

                // Break if the next iteration goes above max.
                // Since we calculate iteratively, this means we already did this many iterations minus 1.
                if (iterations >= maxIterations)
                {
                    DebugConsole.Log($"Max iterations hit. Delta: {delta}");
                    break;
                }

                // Compute distance between the 2 sampled points on the curve 'iterations' times
                currDistance = GetDistanceBetween(evaluable, timeStart, timeEnd, iterations);
                delta = math.abs(currDistance - prevDistance);
                prevDistance = currDistance;
            }
            // Continue this process so long as the distance gained from more precise sampling is more than 'minDelta'
            while (delta >= minDelta);

            //DebugConsole.Log($"iters: {(int)math.pow(powerBase, powerExp)}, distance: {currDistance}");

            // If we stop, 'currDistance' holds the most precise distance value
            return currDistance;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeStart"></param>
        /// <param name="timeEnd"></param>
        /// <param name="nIterations"></param>
        /// <returns></returns>
        public static double GetDistanceBetween(IPositionEvaluable evaluable, double timeStart, double timeEnd, int nIterations)
        {
            // If we start at, say, 0.3 and end at 0.45, get the difference or total "step"
            double timeDelta = timeEnd - timeStart;

            //
            double inverseNIterations = 1.0 / nIterations;

            double distance = 0f;
            for (int i = 0; i <= nIterations; i++)
            {
                // Get the curve times for current + increment towards next checkpoint
                // Each sampling begins at 'timeStart' (for example, 0.3 / 1.0)
                // Each sampling ends at 'timeEnd' (for example, 0.45 / 1.0)
                // We sample from that start to the next iteration. For instance, if we
                // iterate 100 times, and the start is from 0.3000 to 0.3015 since we
                // will travel 0.15s through time from start to end, and each iteration
                // is 1/100.
                double currDistance = timeStart + (timeDelta * inverseNIterations * (i + 0));
                double nextDistance = timeStart + (timeDelta * inverseNIterations * (i + 1));
                // Compute the distance between these 2 points
                float3 currPosition = evaluable.EvaluatePosition(currDistance);
                float3 nextPosition = evaluable.EvaluatePosition(nextDistance);
                // Get distance between 2 points, store delta
                double delta = math.distance(currPosition, nextPosition);
                distance += delta;
            }
            return distance;
        }

        public static float3 CleanRotation(float3 lastEulers, float3 currEulers, float minDelta = 180f)
        {
            var x = CleanRotation(lastEulers.x, currEulers.x, minDelta);
            var y = CleanRotation(lastEulers.y, currEulers.y, minDelta);
            var z = CleanRotation(lastEulers.z, currEulers.z, minDelta);
            return new float3(x, y, z);
        }

        public static float CleanRotation(float lastAngle, float currAngle, float minDelta = 180f)
        {
            //const float minDelta = 180f;
            float delta = currAngle - lastAngle;

            if (delta > minDelta)
            {
                currAngle -= minDelta * 2;
            }
            else if (delta < -minDelta)
            {
                currAngle += minDelta * 2;
            }

            return currAngle;
        }

    }

}
