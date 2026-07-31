using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class BoardBridge: MonoBehaviour
{
    Entity boardEntity;
    EntityManager boss;
    float boardRadius;

    void Start()
    {
        boss = World.DefaultGameObjectInjectionWorld.EntityManager;

        boardEntity = boss.CreateEntity(typeof(BoardTransform));

        Mesh mesh = GetComponent<MeshFilter>().sharedMesh;
        boardRadius = mesh.bounds.extents.x * transform.localScale.x;
    }

    void FixedUpdate()
    {
        boss.SetComponentData(boardEntity, new BoardTransform
        {
            Rotation = transform.rotation,
            Position = transform.position,
            Radius = boardRadius
        });
    }

    void OnDestroy()
    {
        boss.DestroyEntity(boardEntity);
    }
}

public struct BoardTransform : IComponentData
{
    public float3 Position;
    public quaternion Rotation;
    public float Radius;
}
