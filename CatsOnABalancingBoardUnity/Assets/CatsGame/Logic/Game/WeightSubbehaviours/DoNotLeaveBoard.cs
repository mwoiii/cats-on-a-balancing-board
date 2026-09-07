using OMC;
using OMC.ECS;
using Unity.VisualScripting;
using UnityEngine;

public class DoNotLeaveBoard : WeightSubBehaviourBase
{
    public float maxRadiusOffset = 0.1f;
    Rigidbody body;
    BoardController boardController;
    Vector3 boardOrigin;

    public override void Start()
    {
        base.Start();
        
        body = GetComponent<Rigidbody>();
        boardController = BoardController.instance;
        boardOrigin = boardController.transform.position;
    }

    void FixedUpdate()
    {
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

