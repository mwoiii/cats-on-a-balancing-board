using UnityEngine;
using UnityEngine.UI;

public class SpriteFlicker : MonoBehaviour {

    public Sprite[] sprites;

    public Image targetGraphic;

    private const float spriteSwapWait = 0.3f;

    private float spriteStopwatch;

    private int spriteIndex;

    public void Update() {
        spriteStopwatch += Time.deltaTime;
        if (spriteStopwatch > spriteSwapWait) {
            spriteIndex = (spriteIndex + 1) % sprites.Length;
            targetGraphic.sprite = sprites[spriteIndex];
            spriteStopwatch = 0f;
        }
    }
}
