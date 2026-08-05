using System;
using Unity.Entities;

namespace OMC.ECS {
    public partial struct CatCountBridgingSystem : ISystem {
        public static event Action<int> CatCountChange;
        public static event Action<int, DynamicBuffer<LostCatPosition>> CatCountChangePositions;

        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<CatCount>();
            state.RequireForUpdate<LostCatPosition>();
            state.RequireForUpdate<CatSpawnerConfig>();
        }

        public void OnUpdate(ref SystemState state) {
            var catCount = SystemAPI.GetSingletonRW<CatCount>();
            var difference = catCount.ValueRO.gained - catCount.ValueRO.lost;
            if (difference != 0) {
                CatCountChange?.Invoke(difference);
                CatCountChangePositions?.Invoke(difference, SystemAPI.GetSingletonBuffer<LostCatPosition>());
            }
            catCount.ValueRW.gained = 0;
            catCount.ValueRW.lost = 0;
        }
    }
}
