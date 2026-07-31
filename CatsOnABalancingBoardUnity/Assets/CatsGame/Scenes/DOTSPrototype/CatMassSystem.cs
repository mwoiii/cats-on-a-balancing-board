using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

[UpdateAfter(typeof(CatMovementSystem))]
[BurstCompile]
public partial struct CatMassSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<CatMassSnapshot>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float2 weightedSum =  float2.zero;
        float totalMass = 0;

        foreach (var catData in SystemAPI.Query<RefRO<CatData>>().WithDisabled<IsInitialFalling>())
        {
            weightedSum += catData.ValueRO.Position * catData.ValueRO.Mass;
            totalMass += catData.ValueRO.Mass;
        }

        float2 center = totalMass > 0 ? weightedSum / totalMass : float2.zero;
        SystemAPI.SetSingleton(new CatMassSnapshot {CenterOfMass = center, TotalMass = totalMass});
    }
}

public struct CatMassSnapshot : IComponentData
{
    public float2 CenterOfMass; // projected coordinate system
    public float TotalMass;
}
