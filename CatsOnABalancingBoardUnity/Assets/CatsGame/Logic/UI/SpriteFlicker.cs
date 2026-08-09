using UnityEngine;
using UnityEngine.UI;

namespace OMC.UI {
    [RequireComponent(typeof(Image))]
    public class SpriteFlicker : MonoBehaviour {

        public Sprite[] sprites;

        private Image targetGraphic;

        private const float spriteSwapWait = 0.3f;

        private float spriteStopwatch;

        private int spriteIndex;

        [SerializeField]
        private bool unscaledTime = false;

        public void Awake() {
            targetGraphic = GetComponent<Image>();
        }

        public void Update() {
            if (!targetGraphic) {
                return;
            }

            spriteStopwatch += unscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            if (spriteStopwatch > spriteSwapWait) {
                spriteIndex = (spriteIndex + 1) % sprites.Length;
                targetGraphic.sprite = sprites[spriteIndex];
                spriteStopwatch = 0f;
            }
        }
    }
}
