using System.Runtime.CompilerServices;

namespace OMC.Util {
    public static class Helpers {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Mod(int a, int b) {
            int r = a % b;
            return r < 0 ? r + b : r;
        }
    }
}
