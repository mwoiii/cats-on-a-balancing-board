using OMC;
using OMC.ECS;
using Unity.VisualScripting;
using UnityEngine;

public class TheIntelligentMarble : WeightSubBehaviourBase
{
    public float force = 5f;
    public float dampingFactor = 2f;
    public float relativeDistanceFactor = 1.5f;
    public float slopeTolerance = 0.1f;

    public Texture altTexture;

    Rigidbody body;

    BoardController boardController;
    Vector3 boardOrigin;
    CatMassBridge catMassBridge;

    public override void Start()
    {
        base.Start();
        
        body = GetComponent<Rigidbody>();
        boardController = BoardController.instance;
        boardOrigin = boardController.transform.position;
        catMassBridge = CatMassBridge.instance;

        if (Random.Range(0,1f) > 0.99f && TryGetComponent<MeshRenderer>(out var a) && altTexture)
        {
            a.material.SetTexture("_BaseMap",altTexture);
        }
    }

    void FixedUpdate()
    {
        if (!catMassBridge || !boardController || catMassBridge.mass == 0)
        {
            return;
        }

        Vector3 toCOM = catMassBridge.worldPoint - boardOrigin;
        float r = relativeDistanceFactor * toCOM.magnitude;
        Vector3 target = r * (boardController.slope > slopeTolerance ? -boardController.slopeDir : (boardOrigin - toCOM).normalized);
        Vector3 toTarget = Vector3.ProjectOnPlane(target-transform.position,boardController.transform.up);

        Vector3 gravity = 9.81f * boardController.slope * boardController.slopeDir;
        Vector3 pull = toTarget * force;
        Vector3 damping = Vector3.ProjectOnPlane(body.linearVelocity,boardController.transform.up) * dampingFactor;

        body.AddForce(pull - damping - gravity, ForceMode.Acceleration);
    }
}
