using UnityEngine;
using System.Collections;
using OMC;

public class IndecisiveWarp : WeightSubbehaviour
{
    public int warpCount = 4;
    public int warpTime = 1;
    bool indecisiving = false;
    float radius = 3;

    new void Start()
    {
        base.Start();
        if (GameObject.FindGameObjectWithTag("Board").TryGetComponent<Collider>(out Collider a)){radius = a.bounds.extents.x;}
        else{Debug.LogWarning("Hello Indecisive Cube reporting for duty! The board doesn't have a collider component");}
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
                    Vector2 a = UnityEngine.Random.insideUnitCircle * 3;
                    transform.position = new Vector3(a.x, 3, a.y);
                    weightBehaviour.State = WeightBehaviour.WeightState.Falling;
                    yield return new WaitForSeconds(warpTime);
                }
            }
            indecisiving = false;
        }
    }
}
