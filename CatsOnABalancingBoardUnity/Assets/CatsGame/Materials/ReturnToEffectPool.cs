using UnityEngine;
using UnityEngine.Pool;

public class ReturnToEffectPool : MonoBehaviour {
    public IObjectPool<GameObject> pool;

    public float lifetime = 1f;

    private float countdown;

    private void OnEnable() {
        countdown = lifetime;
    }

    private void Update() {
        countdown -= Time.deltaTime;
        if (countdown <= 0f) {
            pool.Release(gameObject);
        }
    }
}
