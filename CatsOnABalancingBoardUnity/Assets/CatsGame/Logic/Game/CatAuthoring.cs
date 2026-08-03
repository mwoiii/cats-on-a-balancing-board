using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

public class CatAuthoring : MonoBehaviour {
    public float mass = 0.1f;

    class Baker : Baker<CatAuthoring> {
        public override void Bake(CatAuthoring authoring) {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new CatData { position = float2.zero, mass = authoring.mass, prevStackIndex = -1 });
            AddComponent(entity, new CatVelocity { value = float2.zero });
            AddComponent(entity, new IsInitialFalling());
            SetComponentEnabled<IsInitialFalling>(entity, true);
            AddComponent(entity, new URPMaterialPropertyBaseColor { Value = new float4(0, 0, 0, 1) });
        }
    }
}

public struct CatData : IComponentData {
    public float2 position;
    public float mass;
    public int prevStackIndex;
}

public struct CatVelocity : IComponentData {
    public float2 value;
}

public struct IsInitialFalling : IComponentData, IEnableableComponent { }
