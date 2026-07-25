using System.Collections.Generic;
using UnityEngine;

public class RandomPawZone : MonoBehaviour {
    private static List<RectTransform> zones = new List<RectTransform>();

    public static void ResetZones() {
        zones.Clear();
    }

    public void Start() {
        if (TryGetComponent(out RectTransform rectTransform)) {
            zones.Add(rectTransform);
        }
    }

    public static Vector3 GetRandomZonePosition() {
        if (zones.Count <= 0) {
            return Vector3.one * -1f;
        }
        RectTransform randomZone = zones[Random.Range(0, zones.Count)];
        return new Vector3(
            Random.Range(randomZone.position.x - randomZone.rect.width * 0.5f, randomZone.position.x + randomZone.rect.width * 0.5f),
            Random.Range(randomZone.position.y - randomZone.rect.height * 0.5f, randomZone.position.y + randomZone.rect.height * 0.5f),
            0f
        );
    }
}
