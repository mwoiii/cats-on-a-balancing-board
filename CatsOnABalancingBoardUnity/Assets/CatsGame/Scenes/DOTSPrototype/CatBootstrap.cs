using System;
using System.Net.Sockets;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class CatBootstrap : MonoBehaviour
{
    public Material catMaterial;
    public Transform boardTransform;
    public int count = 10000;
    public float radius = 5f;
    public float speed = 0.5f;
    public float scale = 0.1f;
    public float dropHeight = 0f;

    void Start()
    {
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        GameObject tempQuad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        Mesh quadMesh = tempQuad.GetComponent<MeshFilter>().sharedMesh;
        Destroy(tempQuad);

        RenderMeshDescription description = new RenderMeshDescription(ShadowCastingMode.Off,receiveShadows: false);
        RenderMeshArray array = new RenderMeshArray(new[] {catMaterial}, new[] {quadMesh});

        var tomPrototypeMini = entityManager.CreateEntity();
        RenderMeshUtility.AddComponents(
            tomPrototypeMini,
            entityManager,
            description,
            array,
            MaterialMeshInfo.FromRenderMeshArrayIndices(0,0));
        
        entityManager.AddComponentData(tomPrototypeMini, LocalTransform.Identity);
        entityManager.AddComponentData(tomPrototypeMini, new LocalToWorld());
        entityManager.AddComponentData(tomPrototypeMini, new CatVelocity());

        Unity.Mathematics.Random randomSauce = new(676767);
        NativeArray<Entity> cats = entityManager.Instantiate(tomPrototypeMini,count,Allocator.Temp);
        float3 boardCenter = boardTransform.position;

        foreach (var cat in cats)
        {
            float2 offset = randomSauce.NextFloat2Direction() * randomSauce.NextFloat(0,radius);
            float3 spawnPos = new(boardCenter.x + offset.x,boardCenter.y + dropHeight,boardCenter.z + offset.y);
            LocalTransform catTransform = new()
            {
                Position = spawnPos,
                Rotation = quaternion.identity,
                Scale = scale
            };

            entityManager.SetComponentData(cat, catTransform);
            entityManager.SetComponentData(cat, new CatVelocity{value = randomSauce.NextFloat2Direction() * speed});
        }

        cats.Dispose();
        entityManager.DestroyEntity(tomPrototypeMini);
    }

    void Update()
    {
        
    }
}

public struct CatVelocity : IComponentData
{
    public float2 value;
}
