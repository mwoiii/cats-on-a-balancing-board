using OMC;
using OMC.ECS;
using Unity.VisualScripting;
using UnityEngine;

public class DoNotLeaveBoard : WeightSubBehaviourBase
{
    public float maxRadiusOffset = 0.1f;
    public float speedLimit = 10f;
    Rigidbody body;
    BoardController boardController;
    Vector3 boardOrigin;
    bool hasEscaped = false;

    public override void Start()
    {
        base.Start();
        
        body = GetComponent<Rigidbody>();
        boardController = BoardController.instance;
        boardOrigin = boardController.transform.position;
    }

    void FixedUpdate()
    {
        if (hasEscaped){return;}

        Vector3 fromBoardOrigin = Vector3.ProjectOnPlane(transform.position - boardOrigin, boardController.transform.up);
        float R = boardController.radius - maxRadiusOffset;
        if (fromBoardOrigin.magnitude > R)
        {
            float speed = Vector3.Dot(body.linearVelocity,fromBoardOrigin.normalized);
            if (speed <= speedLimit)
            {
                Vector3 clamped = boardOrigin + fromBoardOrigin.normalized * R;
                body.MovePosition(new Vector3(clamped.x,transform.position.y,clamped.z));
                body.linearVelocity = Vector3.ProjectOnPlane(body.linearVelocity, fromBoardOrigin.normalized);
            } else
            {
                hasEscaped = true;
            }
        }
    }
}

