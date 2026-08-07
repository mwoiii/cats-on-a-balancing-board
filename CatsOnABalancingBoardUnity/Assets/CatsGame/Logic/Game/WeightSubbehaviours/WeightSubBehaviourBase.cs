using OMC;
using UnityEngine;

[RequireComponent(typeof(WeightBehaviour))]
public class WeightSubBehaviourBase : MonoBehaviour {
    [HideInInspector]
    public WeightBehaviour weightBehaviour;

    public virtual void Start() {
        weightBehaviour = gameObject.GetComponent<WeightBehaviour>();
        if (!weightBehaviour) {
            Debug.LogError($"WeightSubBehaviour component on {gameObject.name} is missing a WeightBehaviour! This is not allowed!");
            Destroy(this);
        }
    }
}
