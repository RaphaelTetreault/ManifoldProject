using System.Runtime.CompilerServices;
using UnityEngine;

namespace GameCube.FZeroGX
{
    public static class AvEditorUtil
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void InvertX(ref Vector3 value)
        {
            value.x = -value.x;
        }

        public static Vector3 InvertX(Vector3 value)
        {
            value.x = -value.x;
            return value;
        }
    }
}