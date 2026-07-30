using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class BoardBridge: MonoBehaviour
{
    Entity boardEntity;
    EntityManager boss;

    void Start()
    {
        boss = World.DefaultGameObjectInjectionWorld.EntityManager;

        boardEntity = boss.CreateEntity(typeof(BoardTransform));
    }

    void FixedUpdate()
    {
        boss.SetComponentData(boardEntity, new BoardTransform
        {
            Rotation = transform.rotation,
            Position = transform.position
        });
    }
}

public struct BoardTransform : IComponentData
{
    public float3 Position;
    public quaternion Rotation;
}
