using OMC;
using OMC.ECS;
using Unity.VisualScripting;
using UnityEngine;

public class OpposeCatMass : WeightSubBehaviourBase
{
    public float force = 5f;
    public float dampingFactor = 2f;
    public float relativeMassFactor = 2;
    public float relativeMassMinimum = 10;
    public float relativeDistanceFactor = 1.5f;
    public float maxRadiusOffset = 0.1f;

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
        if (weightBehaviour.state == OMC.WeightBehaviour.WeightState.Falling || !catMassBridge || !boardController || catMassBridge.mass == 0)
        {
            return;
        }

        body.mass = Mathf.Max(relativeMassMinimum,relativeMassFactor * catMassBridge.mass);


        float r = relativeDistanceFactor * (catMassBridge.worldPoint - boardOrigin).magnitude;
        Vector3 target = r * -boardController.slopeDir;
        Vector3 toTarget = Vector3.ProjectOnPlane(target-transform.position,boardController.transform.up);

        Vector3 gravity = 9.81f * boardController.slope * boardController.slopeDir;
        Vector3 pull = toTarget * force;
        Vector3 damping = Vector3.ProjectOnPlane(body.linearVelocity,boardController.transform.up) * dampingFactor;

        body.AddForce(pull - damping - gravity, ForceMode.Acceleration);

        // board radius clamping
        Vector3 fromBoardOrigin = Vector3.ProjectOnPlane(transform.position - boardOrigin, boardController.transform.up);
        float R = boardController.radius - maxRadiusOffset;
        if (fromBoardOrigin.magnitude > R)
        {
            Vector3 clamped = boardOrigin + fromBoardOrigin.normalized * R;
            body.MovePosition(new Vector3(clamped.x,transform.position.y,clamped.z));
            body.linearVelocity = Vector3.ProjectOnPlane(body.linearVelocity, fromBoardOrigin.normalized);
        }
    }
}
