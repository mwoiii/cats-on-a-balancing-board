using System.Linq;
using UnityEngine;

namespace OMC.UI {
    public class MenuThemeSyncPaws : MonoBehaviour {
        public AudioSource song;

        public float[] meowTimestamps;

        float lastTimestamp;

        int next;

        bool lastMeow = false;

        void Start() {
            lastTimestamp = meowTimestamps.Last();
        }

        void Update() {
            if (song == null || !song.isPlaying) { return; }

            if (lastMeow && song.time < lastTimestamp) {
                lastMeow = false;
            }

            if (!lastMeow && song.time >= meowTimestamps[next]) {
                MenuController.instance.PlaceRandomPaw();
                next++;
                if (next == meowTimestamps.Length) {
                    lastMeow = true;
                    next = 0;
                }
            }
        }
    }
}
