using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

namespace OMC.ECS {
    [BurstCompile]
    [UpdateBefore(typeof(CatCountBridgingSystem))]
    public partial struct CatSpawnSystem : ISystem {
        uint spawnCallCount;

        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<CatSpawnerConfig>();
            state.RequireForUpdate<CatCount>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            CatSpawnerConfig config = SystemAPI.GetSingleton<CatSpawnerConfig>();
            var catCount = SystemAPI.GetSingletonRW<CatCount>();

            int countToSpawn = 0;
            if (!config.finished) {
                countToSpawn = config.count;
                config.finished = true;
            } else if (config.pendingSpawn > 0) {
                countToSpawn = config.pendingSpawn;
                config.pendingSpawn = 0;
            }

            config.spawnedThisUpdate = countToSpawn;

            if (countToSpawn == 0) { return; }

            catCount.ValueRW.gained += countToSpawn;


            int totalEntityTarget = config.batchEntityTarget;

            int topTier = 0;
            while (countToSpawn / TomNumber(topTier) > totalEntityTarget) {
                topTier++;
            }

            int tierCount = topTier + 1;
            NativeArray<int> valuedEntityCounts = new(tierCount, Allocator.Temp);
            int remaining = countToSpawn;

            if (topTier > 0) { // we newtons method
                int first = TomNumber(topTier);
                int second = TomNumber(topTier - 1);

                int h = (int)((long)countToSpawn - (long)totalEntityTarget * second) / (first - second);
                h = math.clamp(h, 0, countToSpawn / first);

                valuedEntityCounts[topTier] = h;
                remaining -= h * first;
            }

            for (int i = topTier > 0 ? topTier - 1 : topTier; i >= 0; i--) { // I love programming!!!!!!!!
                int t = TomNumber(i);
                valuedEntityCounts[i] = remaining / t;
                remaining %= t;
            }

            int entitiesToSpawn = 0;
            for (int i = 0; i < tierCount; i++) {
                entitiesToSpawn += valuedEntityCounts[i];
            }

            // kills burst but useful
            //string debugstring = "";
            //for (int i = 0; i < valuedEntityCounts.Length; i++)
            //{
            //    debugstring += $"{valuedEntityCounts[i]} of value {TomNumber(i)}, ";
            //}
            //Debug.Log(debugstring + $"for a total of {entitiesToSpawn} entities");
            //

            catCount.ValueRW.gainedRaw += entitiesToSpawn;

            Unity.Mathematics.Random randomSauce = new(676767 + spawnCallCount);
            spawnCallCount++;

            NativeArray<Entity> cats = state.EntityManager.Instantiate(config.prefab, entitiesToSpawn, Allocator.Temp);

            int currentValueIndex = 0;
            int cum = valuedEntityCounts[0];
            for (int i = 0; i < cats.Length; i++) {
                Entity cat = cats[i];

                while (i >= cum && currentValueIndex < valuedEntityCounts.Length) {
                    currentValueIndex++;
                    cum += valuedEntityCounts[currentValueIndex];
                }

                CatValue value = SystemAPI.GetComponent<CatValue>(cat);
                value.value = TomNumber(currentValueIndex);
                SystemAPI.SetComponent(cat, value);

                float2 offset = config.radius * math.sqrt(randomSauce.NextFloat(0, 1)) * randomSauce.NextFloat2Direction();
                CatData data = SystemAPI.GetComponent<CatData>(cat);
                data.position = offset;
                state.EntityManager.SetComponentData(cat, data);

                LocalTransform catTransform = LocalTransform.FromPositionRotationScale(
                new float3(offset.x, config.dropHeight, offset.y), quaternion.identity, config.scale);
                state.EntityManager.SetComponentData(cat, catTransform);

                // Spribnkes...
                //float3 hugh = HSVToRGBBurstable(randomSauce.NextFloat(), 1f, 1f);
                
                // Realistic cat colouring
                //float3 hugh = RandomRealCatColour(randomSauce);
                
                //state.EntityManager.SetComponentData(cat, new URPMaterialPropertyBaseColor { Value = new float4(hugh, 1f) });
                
                SystemAPI.SetSingleton(new InitialFallData { height = config.dropHeight, velocity = 0 });
            }

            SystemAPI.SetSingleton(config);
        }

        static float3 HSVToRGBBurstable(float h, float s, float v) {
            float3 p = math.abs(math.frac(h + new float3(1f, 2f / 3f, 1f / 3f)) * 6f - 3f);
            float3 rgb = math.saturate(p - 1f);
            return v * math.lerp(new float3(1f), rgb, s);
        }

        static int TomNumber(int a) {
            if (a <= 0) { return 1; } else if (a == 1) { return 7; } else {
                int t = 1;
                for (int i = 0; i < a - 1; i++) { t *= 10; }
                return t + 7;
            }
        }

        static int CountDigitsBurstable(int value) {
            if (value == 0) { return 1; }
            value = math.abs(value);
            int digits = 0;
            while (value > 0) {
                digits++;
                value /= 10;
            }
            return digits;
        }

        static float3 RandomRealCatColour(Unity.Mathematics.Random randomSauce)
        {
            float a = randomSauce.NextFloat(0,68.8f);
            if (a < 26.5f)
            {
                return new float3(0,0,0);
            }
            else if (a < 45.9)
            {
                return new float3(100,100,100)/255;
            }
            else if (a < 58.2)
            {
                return new float3(90,45,0)/255;
            }
            else
            {
                return new float3(240,120,0)/255;
            }
        }
    }
}
