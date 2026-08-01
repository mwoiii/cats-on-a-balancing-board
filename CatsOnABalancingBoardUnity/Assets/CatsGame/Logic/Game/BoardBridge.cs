using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class BoardBridge : MonoBehaviour {
    Entity boardEntity;

    EntityManager boss;

    float boardRadius;

    void Start() {
        boss = World.DefaultGameObjectInjectionWorld.EntityManager;

        EntityQuery query = boss.CreateEntityQuery(typeof(BoardTransform));
        if (!query.TryGetSingletonEntity<BoardTransform>(out boardEntity)) {
            boardEntity = boss.CreateEntity(typeof(BoardTransform));
        }

        Mesh mesh = GetComponent<MeshFilter>().sharedMesh;
        boardRadius = mesh.bounds.extents.x * transform.localScale.x;
    }

    void FixedUpdate() {
        boss.SetComponentData(boardEntity, new BoardTransform {
            rotation = transform.rotation,
            position = transform.position,
            radius = boardRadius
        });
    }

    void OnDestroy() {
        boss.DestroyEntity(boardEntity);
    }
}

public struct BoardTransform : IComponentData {
    public float3 position;
    public quaternion rotation;
    public float radius;
}
