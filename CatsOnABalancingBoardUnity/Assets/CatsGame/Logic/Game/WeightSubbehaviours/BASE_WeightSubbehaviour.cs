using UnityEngine;
using OMC;

[RequireComponent(typeof(WeightBehaviour))]
public class WeightSubbehaviour : MonoBehaviour
{
    [HideInInspector]
    public WeightBehaviour weightBehaviour;

    internal void Start()
    {
        weightBehaviour = gameObject.GetComponent<WeightBehaviour>();
        if (weightBehaviour == null){Debug.LogError("HOW");}
    }
}
