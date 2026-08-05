using OMC.ECS;
using Unity.Collections;
using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public class CatBatchDeletion : MonoBehaviour {
    [SerializeField]
    private float period = 10f;

    private float countdown;

    private EntityManager entityManager;

    private EntityQuery query;

    private void Start() {
        entityManager = StaticEntityData.entityManager;
        countdown = period;
        query = entityManager.CreateEntityQuery(ComponentType.ReadOnly<FallenCatData>(), ComponentType.ReadOnly<LocalTransform>(), ComponentType.Exclude<MaterialMeshInfo>());
    }

    private void Update() {
        countdown -= Time.deltaTime;
        if (countdown <= 0) {
            ExecuteBatchDeletion();
            countdown = period;
        }
    }

    private void ExecuteBatchDeletion() {
        if (!query.IsEmpty) {
            var array = query.ToEntityArray(Allocator.TempJob);
            // experimenting with tradeoff for single lagspike but instant memory clear or lots of smaller lagspikes with gradual memory tradeoff
            // hard to know if latter is perceptible so running with prior until then
            //int quantity = math.max(1000, (int)(array.Length * 0.5f));
            entityManager.DestroyEntity(array);//array.GetSubArray(0, quantity));
        }
    }
}
