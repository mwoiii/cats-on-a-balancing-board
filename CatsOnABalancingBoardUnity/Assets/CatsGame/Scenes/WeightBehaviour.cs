using UnityEngine;

public class WeightBehaviour : MonoBehaviour
{
    public enum WeightType {None,Catnip}
    public WeightType Type = WeightType.None;

    public enum WeightState {Falling,Landed}
    public WeightState State {get; private set;} = WeightState.Falling;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        if (State == WeightState.Falling){State=WeightState.Landed;}
    }
}
