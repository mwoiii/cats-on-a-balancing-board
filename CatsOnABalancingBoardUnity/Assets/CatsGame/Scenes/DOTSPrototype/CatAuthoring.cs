using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class CatAuthoring : MonoBehaviour
{
    public float mass = 0.1f;
    class Baker : Baker<CatAuthoring>
    {
        public override void Bake(CatAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new CatData{Position = float2.zero, Mass = authoring.mass});
            AddComponent(entity, new CatVelocity {Value = float2.zero});
        }
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}

public struct CatData: IComponentData
{
    public float2 Position;
    public float Mass;
}

public struct CatVelocity: IComponentData
{
    public float2 Value;
}
