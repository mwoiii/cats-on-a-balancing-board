using UnityEngine;
using System.Collections;
using OMC;

public class IndecisiveWarp : WeightSubbehaviour
{
    public int warpCount = 4;
    public int warpTime = 1;
    bool indecisiving = false;

    public float selfRadiusRelative = 1/3;
    float boardRadius = 3;
    float selfRadius;

    new void Start()
    {
        base.Start();
        if (GameObject.FindGameObjectWithTag("Board").TryGetComponent<Collider>(out Collider a)){boardRadius = a.bounds.extents.x;}
        else{Debug.LogWarning("Hello Indecisive Cube reporting for duty! The board doesn't have a collider component");}
        selfRadius = boardRadius * selfRadiusRelative;
    }

    void OnCollisionEnter(Collision collision)
    {
        StartCoroutine(Warp());
    }

    IEnumerator Warp() {
        if (!indecisiving)
        {
            indecisiving = true;

            yield return new WaitForSeconds(warpTime);
            for (int i = 0; i < warpCount; i++)
            {
                if (transform != null)
                {
                    Vector2 a = new(transform.position.x,transform.position.z);
                    Vector2 b = UnityEngine.Random.insideUnitCircle * boardRadius;
                    while (Vector2.Distance(a,b) > selfRadius)
                    {
                        b = UnityEngine.Random.insideUnitCircle * boardRadius;
                    }
                    transform.position = new Vector3(b.x, 3, b.y);
                    weightBehaviour.State = WeightBehaviour.WeightState.Falling;
                    yield return new WaitForSeconds(warpTime);
                }
            }
            indecisiving = false;
        }
    }
}
