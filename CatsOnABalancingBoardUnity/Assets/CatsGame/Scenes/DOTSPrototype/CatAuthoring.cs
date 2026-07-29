using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class CatAuthoring : MonoBehaviour
{
    class Baker : Baker<CatAuthoring>
    {
        public override void Bake(CatAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<CatTag>(entity);
        }
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}

public struct CatTag: IComponentData{}
