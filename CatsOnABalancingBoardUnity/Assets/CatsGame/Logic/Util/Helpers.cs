using System.Runtime.CompilerServices;
using UnityEngine;

namespace OMC.Util {
    public static class Helpers {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Mod(int a, int b) {
            int r = a % b;
            return r < 0 ? r + b : r;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 GetRandomShake(float intensity) {
            return (Vector3.up * Random.Range(-1f, 1f) + Vector3.right * Random.Range(-1f, 1f)) * intensity;
        }
    }
}
