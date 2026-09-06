using System.IO;
using OMC;
using OMC.ECS;
using UnityEngine;

public class OpposeCatMass : WeightSubBehaviourBase
{
    public float force = 5f;
    public float dampingFactor = 2f;
    public float relativeMassFactor = 2;
    public float relativeMassMinimum = 10;
    public float maxRadiusOffset = 0.1f;
    public float boardSlopeTolerance = 0.05f;

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
    }

    void FixedUpdate()
    {
        if (weightBehaviour.state != OMC.WeightBehaviour.WeightState.Landed || !catMassBridge || !boardController || catMassBridge.mass == 0 || boardSlopeTolerance < 0.05f)
        {
            return;
        }

        body.mass = Mathf.Min(relativeMassMinimum,relativeMassFactor * catMassBridge.mass);

        Vector3 tangentDir = Vector3.Cross(boardController.slopeDir,boardController.transform.up).normalized;

        Vector3 toCOM = catMassBridge.worldPoint - boardOrigin;

        float slopeComponent = Vector3.Dot(toCOM,boardController.slopeDir);
        float tangentComponent = Vector3.Dot(toCOM, tangentDir);

        Vector3 target = boardOrigin + tangentDir * tangentComponent - boardController.slopeDir * slopeComponent;
        Vector3 toTarget = Vector3.ProjectOnPlane(target-transform.position,boardController.transform.up);

        Vector3 gravity = 9.81f * boardController.slope * boardController.slopeDir;
        Vector3 pull = toTarget * force;
        Vector3 damping = Vector3.ProjectOnPlane(body.linearVelocity,boardController.transform.up) * dampingFactor;

        body.AddForce(pull - damping - gravity, ForceMode.Acceleration);

        // board radius clamping
        Vector3 fromBoardOrigin = Vector3.ProjectOnPlane(transform.position - boardOrigin, boardController.transform.up);
        float r = boardController.radius - maxRadiusOffset;
        if (fromBoardOrigin.magnitude > r)
        {
            Vector3 clamped = boardOrigin + fromBoardOrigin.normalized * r;
            transform.position = new Vector3(clamped.x,transform.position.y,clamped.z);
            body.linearVelocity = Vector3.ProjectOnPlane(body.linearVelocity, fromBoardOrigin.normalized);
        }
    }
}
