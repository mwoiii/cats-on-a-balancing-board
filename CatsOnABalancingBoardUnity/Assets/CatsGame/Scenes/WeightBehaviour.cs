using UnityEngine;

public class WeightBehaviour : MonoBehaviour
{
    public enum WeightType { None, Catnip, Lemon }
    public WeightType Type = WeightType.None;

    public enum WeightState { Falling, Landed }
    public WeightState State { get; private set; } = WeightState.Falling;

    [SerializeField] private float shrinkAmount = 0.01f;
    [SerializeField] private float minScale = 0.01f;
    [SerializeField] private string catTag = "Cat";
    [SerializeField] private float shrinkInterval = 0.5f; // seconds between shrink ticks

    private float shrinkTimer = 0f;

    void Start()
    {

    }

    void Update()
    {
        if (shrinkTimer > 0f)
        {
            shrinkTimer -= Time.deltaTime;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (State == WeightState.Falling) { State = WeightState.Landed; }
    }

    void OnCollisionStay(Collision collision)
    {
        if (Type != WeightType.Catnip) return;
        if (!collision.collider.CompareTag(catTag)) return;
        if (shrinkTimer > 0f) return; // still on cooldown

        ShrinkAndCheck();
        shrinkTimer = shrinkInterval;
    }

    private void ShrinkAndCheck()
    {
        Vector3 newScale = transform.localScale - Vector3.one * shrinkAmount;
        transform.localScale = newScale;

        if (newScale.x < minScale || newScale.y < minScale || newScale.z < minScale)
        {
            Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        WeightDropper.weightBehaviourDict.Remove(transform.gameObject);
    }
}