using UnityEngine;


namespace OMC {
    [CreateAssetMenu(menuName = "OMC/EffectDef")]
    public class EffectDef : ScriptableObject {
        public GameObject effectPrefab;

        [Tooltip("Minimum duration that an effect is allowed to remain unused in the effect pool for before it is destroyed")]
        public float staleTimeBeforeCull = 30f;

        [Tooltip("How many inactive effects are allowed to exist in the pool at once")]
        public int maxPoolSize = 100;

        [Tooltip("How many active effects are allowed to exist at once. If the pool is full, objects will be destroyed instead of released")]
        public int maxEffectQuantity = 100;

        [Tooltip("Whether or not new effects should be prioritised in the event that the maximum effect quantity is reached. Prematurely ends the oldest active effect")]
        public bool prioritiseNewEffects = true;
    }
}
