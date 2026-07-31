using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class WeightBridge: MonoBehaviour
{
    public Transform board;

    Entity weightEntity;
    EntityManager boss;
    readonly List<WeightBehaviour> activeWeights = new();

    void Start()
    {
        boss = World.DefaultGameObjectInjectionWorld.EntityManager;
        weightEntity = boss.CreateEntity();
        boss.AddBuffer<WeightSnapshot>(weightEntity);
        boss.AddBuffer<WeightContactPulse>(weightEntity);
    }

    void FixedUpdate()
    {
        DynamicBuffer<WeightContactPulse> pulses = boss.GetBuffer<WeightContactPulse>(weightEntity);
        for (int i = 0; i < pulses.Length && i < activeWeights.Count; i++)
        {
            if (pulses[i].Count > 0 && activeWeights[i] != null)
            {
                activeWeights[i].NotifyCatContact();
            }
        }

        DynamicBuffer<WeightSnapshot> buffer = boss.GetBuffer<WeightSnapshot>(weightEntity);
        buffer.Clear();
        pulses.Clear();
        activeWeights.Clear();

        foreach (KeyValuePair<GameObject, WeightBehaviour> entry in WeightDropper.weightBehaviourDict)
        {
            GameObject weightObject = entry.Key;
            WeightBehaviour behaviour = entry.Value;
            if (weightObject == null || behaviour == null) {continue;} // Guy who only just learnt what continue does

            Vector3 worldOffset = weightObject.transform.position - board.position;
            Vector3 localPos = Quaternion.Inverse(board.rotation) * worldOffset;
            buffer.Add(new WeightSnapshot{LocalPosition = new float2(localPos.x,localPos.z), Type = behaviour.Type, State = behaviour.State });
            pulses.Add(new WeightContactPulse {Count = 0});
            activeWeights.Add(behaviour);
        }
    }

    void OnDestroy()
    {
        boss.DestroyEntity(weightEntity);
    }
}

public struct WeightSnapshot : IBufferElementData
{
    public float2 LocalPosition;
    public WeightBehaviour.WeightType Type;
    public WeightBehaviour.WeightState State;
}

public struct WeightContactPulse : IBufferElementData
{
    public int Count;
}
