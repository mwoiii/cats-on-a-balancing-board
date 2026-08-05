using UnityEngine;

namespace OMC.UI {
    public class FPSCounter : MonoBehaviour {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
        public float updateInterval = 0.5f;

        float accumTime;
        int frames;
        float fps;
        GUIStyle style;

        void Awake() {
            DontDestroyOnLoad(gameObject);
        }

        void Update() {
            frames++;
            accumTime += Time.unscaledDeltaTime;
            if (accumTime >= updateInterval) {
                fps = frames / accumTime;
                frames = 0;
                accumTime = 0f;
            }
        }

        void OnGUI() {
            style ??= new GUIStyle(GUI.skin.label) { fontSize = 24, normal = { textColor = Color.blueViolet } };
            GUI.Label(new Rect(10, 10, 200, 40), $"{fps:0.}", style);
        }
#endif
    }
}
