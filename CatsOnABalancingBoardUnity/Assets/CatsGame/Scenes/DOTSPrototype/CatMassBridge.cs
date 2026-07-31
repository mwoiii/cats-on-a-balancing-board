using Unity.Entities;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class CatMassBridge : MonoBehaviour
{
    public Rigidbody board;

    EntityManager boss;
    Entity entity;

    void Start()
    {
        boss = World.DefaultGameObjectInjectionWorld.EntityManager;
        entity = boss.CreateEntity(typeof(CatMassSnapshot));
    }

    void FixedUpdate()
    {
        if (!boss.Exists(entity)){return;}

        CatMassSnapshot snapshot = boss.GetComponentData<CatMassSnapshot>(entity);
        if (snapshot.TotalMass == 0){return;}

        Vector3 localPoint = new Vector3(snapshot.CenterOfMass.x,0,snapshot.CenterOfMass.y);
        Vector3 worldPoint = board.transform.TransformPoint(localPoint);

        float force = snapshot.TotalMass * 9.81f;
        board.AddForceAtPosition(Vector3.down*force, worldPoint, ForceMode.Force);
    }
}
