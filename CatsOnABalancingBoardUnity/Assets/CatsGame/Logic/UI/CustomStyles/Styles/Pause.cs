using System.Collections.Generic;

namespace OMC.UI.CustomStyles {
    internal class Pause : IDelayStyle {
        public string prefix => "pause";

        private Dictionary<int, int> pauseCharacters = new Dictionary<int, int>();

        public void ApplyDelay(int visibleCharacters, ref float cooldown) {
            if (pauseCharacters.TryGetValue(visibleCharacters - 1, out int pauseValue)) {
                cooldown = 0.3333f * pauseValue;
            }
        }

        public void ReceiveStartCharacter(int index, int value, bool broken) {
            pauseCharacters[index] = value;
        }
    }
}
