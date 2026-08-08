using System;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

namespace OMC.ECS {
    [UpdateAfter(typeof(CatProjectionSystem))] // A millisecond is a millisecond
    [BurstCompile]
    public partial struct CatMassSystem : ISystem {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<CatMassSnapshot>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            float2 weightedSum = float2.zero;
            float totalMass = 0;

            foreach (var catData in SystemAPI.Query<RefRO<CatData>>().WithDisabled<IsInitialFalling>()) {
                weightedSum += catData.ValueRO.position * catData.ValueRO.mass;
                totalMass += catData.ValueRO.mass;
            }

            float2 center = totalMass > 0 ? weightedSum / totalMass : float2.zero;
            SystemAPI.SetSingleton(new CatMassSnapshot { centerOfMass = center, totalMass = totalMass });
        }
    }

    public struct CatMassSnapshot : IComponentData {
        public float2 centerOfMass; // projected coordinate system
        public float totalMass;
    }
}
