using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace OMC.ECS {
    public class CatCountDataAuthoring : MonoBehaviour {

        public int arrayLength = 50;

        class Baker : Baker<CatCountDataAuthoring> {
            public override void Bake(CatCountDataAuthoring authoring) {
                Entity entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new CatCount());
                var positionBuffer = AddBuffer<LostCatPosition>(entity);
                positionBuffer.Resize(authoring.arrayLength, NativeArrayOptions.UninitializedMemory);
            }
        }
    }

    public struct CatCount : IComponentData {
        public int gained;
        public int lost;
    }

    public struct LostCatPosition : IBufferElementData {
        public float3 value;
    }
}
