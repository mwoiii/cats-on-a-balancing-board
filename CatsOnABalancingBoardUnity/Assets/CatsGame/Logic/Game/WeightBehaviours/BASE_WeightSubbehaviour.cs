using UnityEngine;
using OMC;

public class WeightSubbehaviour : MonoBehaviour
{
    [HideInInspector]
    public WeightBehaviour weightBehaviour;

    internal void Start()
    {
        weightBehaviour = gameObject.GetComponent<WeightBehaviour>();
        if (weightBehaviour == null){Debug.LogError("Prefab needs WeightBehaviour before sub behaviours can be added");}
    }
}
