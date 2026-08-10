using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

namespace OMC.ECS {
    public class CatAuthoring : MonoBehaviour {
        public float mass = 0.1f;

        class Baker : Baker<CatAuthoring> {
            public override void Bake(CatAuthoring authoring) {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new CatData { position = float2.zero});
                AddComponent(entity, new CatStack { prevStackIndex = -1, smoothStackOffset = 0});
                AddComponent(entity, new CatVelocity { value = float2.zero });
                AddComponent(entity, new CatValue{value = 1});
                AddComponent(entity, new IsInitialFalling());
                SetComponentEnabled<IsInitialFalling>(entity, true);
                AddComponent(entity, new URPMaterialPropertyBaseColor { Value = new float4(0, 0, 0, 1) });
                AddComponent(entity, new FallingCatData());
                SetComponentEnabled<FallingCatData>(entity, false);
                AddComponent(entity, new FallenCatData());
                SetComponentEnabled<FallenCatData>(entity, false);
            }
        }
    }

    public struct CatData : IComponentData, IEnableableComponent {
        public float2 position;
    }

    public struct CatVelocity : IComponentData, IEnableableComponent {
        public float2 value;
    }

    public struct CatStack : IComponentData, IEnableableComponent
    {
        public int prevStackIndex;
        public float smoothStackOffset;
    }

    public struct CatValue: IComponentData, IEnableableComponent
    {
        public int value;
    }

    public struct IsInitialFalling : IComponentData, IEnableableComponent { }
}

