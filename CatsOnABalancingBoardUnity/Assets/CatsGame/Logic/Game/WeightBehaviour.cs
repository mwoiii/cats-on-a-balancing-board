using Assets.CatsGame.Logic.Game;
using System.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class WeightBehaviour : MonoBehaviour {
    public enum WeightType { None, Catnip, Lemon, Antimatter }

    public WeightType type = WeightType.None;

    public enum WeightState { Falling, Landed }

    public WeightState State { get; private set; } = WeightState.Falling;

    [SerializeField]
    private float shrinkAmount = 0.01f;

    [SerializeField]
    private float minScale = 0.01f;

    [SerializeField]
    private string catTag = "Cat";

    [SerializeField]
    private float shrinkInterval = 0.5f; // seconds between shrink ticks

    public float shrinkIntervalLemon = 0.8f;

    private float shrinkTimer = 0f;

    private EntityManager entityManager;

    private void Start() {
        entityManager = StaticEntityData.entityManager;
    }

    void Update() {
        if (shrinkTimer > 0f) {
            shrinkTimer -= Time.deltaTime;
        }
    }

    void OnCollisionEnter(Collision collision) {
        if (State == WeightState.Falling) {
            State = WeightState.Landed;
            if (type == WeightType.Lemon) {
                StartCoroutine(Decay());
            }
        }
        WeightBehaviour a = collision.collider.gameObject.GetComponent<WeightBehaviour>();
        if (a != null) {
            if (a.type == WeightType.Antimatter && type != WeightType.Antimatter) {
                Destroy(a.gameObject);
                Destroy(transform.gameObject);
                AudioPool.instance.PlaySupernovaSoundAt(transform.position);
                var effectConfig = StaticEntityData.effectConfig;
                if (effectConfig.currentSupernovaCount < effectConfig.maxSupernovaCount) {
                    Entity supernova = entityManager.Instantiate(effectConfig.supernovaPrefab);
                    effectConfig.currentSupernovaCount++;
                    entityManager.SetComponentData(supernova, new LocalTransform { Position = transform.position, Scale = 0.2f });
                }
            }
        }
    }

    IEnumerator Decay() {
        while (gameObject != null) {
            ShrinkAndCheck();
            yield return new WaitForSeconds(shrinkIntervalLemon);
        }
    }

    void OnCollisionStay(Collision collision) {
        if (type != WeightType.Catnip) return;
        if (!collision.collider.CompareTag(catTag)) return;
        if (shrinkTimer > 0f) return; // still on cooldown

        ShrinkAndCheck();
        shrinkTimer = shrinkInterval;
    }

    private void ShrinkAndCheck() {
        Vector3 newScale = transform.localScale - Vector3.one * shrinkAmount;
        transform.localScale = newScale;

        if (newScale.x < minScale || newScale.y < minScale || newScale.z < minScale) {
            Destroy(gameObject);
        }
    }

    void OnDestroy() {
        WeightDropper.weightBehaviourDict.Remove(transform.gameObject);
    }

    public void NotifyCatContact() {
        if (type != WeightType.Catnip) { return; }
        if (shrinkTimer > 0) { return; }

        ShrinkAndCheck();
        shrinkTimer = shrinkInterval;
    }
}
