using UnityEngine;
using UnityEngine.EventSystems;

public class CatsMenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler {

    public AudioSource pointerEnterNoise;

    public AudioSource pointerClickNoise;

    public void OnPointerClick(PointerEventData eventData) {
        if (pointerClickNoise) {
            pointerClickNoise.Play();
        }
    }

    public void OnPointerEnter(PointerEventData eventData) {
        if (pointerEnterNoise) {
            pointerEnterNoise.Play();
            MenuController.instance.PlaceRandomPaw();
        }
    }

    public void OnPointerExit(PointerEventData eventData) {

    }
}
