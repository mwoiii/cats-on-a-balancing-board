using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.CatsGame.Logic.Game {
    public static class GlobalTimescale {

        public static float timeScale = 1f;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Init() {
            SceneManager.sceneLoaded += (_, _) => {
                Time.timeScale = timeScale;
            };
        }
    }
}
