using Unity.Entities;
using UnityEngine;

public class CatMassBridge : MonoBehaviour {
    public Rigidbody board;

    EntityManager boss;

    Entity entity;

    void Start() {
        boss = World.DefaultGameObjectInjectionWorld.EntityManager;
        EntityQuery query = boss.CreateEntityQuery(typeof(CatMassSnapshot));
        if (!query.TryGetSingletonEntity<CatMassSnapshot>(out entity)) {
            entity = boss.CreateEntity(typeof(CatMassSnapshot));
        }
    }

    void FixedUpdate() {
        if (!boss.Exists(entity)) {
            return;
        }

        CatMassSnapshot snapshot = boss.GetComponentData<CatMassSnapshot>(entity);
        if (snapshot.totalMass == 0) {
            return;
        }

        Vector3 localPoint = new Vector3(snapshot.centerOfMass.x, 0, snapshot.centerOfMass.y);
        Vector3 worldPoint = board.transform.TransformPoint(localPoint);

        float force = snapshot.totalMass * 9.81f;
        board.AddForceAtPosition(Vector3.down * force, worldPoint, ForceMode.Force);
    }
}
