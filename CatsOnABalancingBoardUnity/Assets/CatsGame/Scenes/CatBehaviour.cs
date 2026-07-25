using System.Collections;
using UnityEngine;

public class CatBehaviour : MonoBehaviour
{
    public float moveForce = 1;
    public float centerBiasForce = 0.1f;
    public float reactDistance = 1.5f;
    public float slopeTolerance = 0.05f;

    public float baseDamping = 3;
    public float gripDamping = 10;

    public float gripTimeMin = 2;
    public float gripTimeMax = 8;
    public float gripCooldown = 5;

    Transform board;
    BoardMath boardMath;
    Rigidbody body;

    void Start()
    {
        body = GetComponent<Rigidbody>();
        
        GameObject boardObject = GameObject.FindGameObjectWithTag("Board");
        board = boardObject.transform;
        boardMath = boardObject.GetComponent<BoardMath>();

        body.linearDamping = baseDamping;
    }

    void FixedUpdate()
    {
        SurvivalInstinct();

        Transform target = FindNearestWeight();
        if (target == null){CenterBias(); return;}

        Vector3 toTarget = target.position - body.transform.position;
        toTarget.y = 0;
        if (toTarget.sqrMagnitude <= 0){CenterBias(); return;}

        Vector3 dir = toTarget.normalized;

        WeightBehaviour ba = target.GetComponent<WeightBehaviour>();
        // THIS IS BEHAVIOUR PRIORITY ORDER FOR OBJECTS
        if (ba.State == WeightBehaviour.WeightState.Falling) // repelled by falling weights
        {
            body.AddForce(dir * -moveForce, ForceMode.Acceleration);
        }
        else if (ba.Type == WeightBehaviour.WeightType.Catnip) // attracted to catnip weights
        {
            body.AddForce(dir * moveForce, ForceMode.Acceleration);
        }
    }

    void CenterBias()
    {
        Vector3 toCenter = board.position - transform.position;
        if (toCenter.sqrMagnitude > 0)
        {
            body.AddForce(centerBiasForce * toCenter.normalized, ForceMode.Acceleration);
        }

    }

    Transform FindNearestWeight() // does not account for y distance
    {
        GameObject[] weights = GameObject.FindGameObjectsWithTag("Weight");
        if (weights.Length == 0){return null;}

        Transform nearest = null;
        float winner = reactDistance;
        
        foreach (GameObject w in weights)
        {
            float dist = Vector2.Distance(new Vector2(transform.position.x,transform.position.z),new Vector2(w.transform.position.x,w.transform.position.z));
            if (dist < winner)
            {
                winner = dist;
                nearest = w.transform;
            }
        }
        return nearest;
    }


    bool gripping = false;
    bool canGrip = true;
    void SurvivalInstinct()
    {
        if (boardMath.slope > slopeTolerance && canGrip)
        {
            body.linearDamping = gripDamping;
            if (!gripping)
            {
                gripping = true;
                StartCoroutine(Grip());
            }
        }
        else{ gripping = false; }
    }

    IEnumerator Grip()
    {
        yield return new WaitForSeconds(Random.Range(gripTimeMin,gripTimeMax));
        body.linearDamping = baseDamping;
        gripping = false;
        canGrip = false;
        yield return new WaitForSeconds(gripCooldown);
        canGrip = true;
    }
}