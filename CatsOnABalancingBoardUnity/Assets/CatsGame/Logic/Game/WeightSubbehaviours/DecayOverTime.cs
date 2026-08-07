using UnityEngine;
using System.Collections;
using OMC;
using UnityEngine.AI;

public class DecayOverTime : WeightSubbehaviour
{
    public float shrinkDelay = 0f;
    public float shrinkInterval = 0.8f;

    void OnCollisionEnter(Collision collision)
    {
        StartCoroutine(Decay());
    }
    
    bool decaying = false;
    IEnumerator Decay() 
    {
        if (!decaying)
        {
            decaying = true;
            yield return new WaitForSeconds(shrinkDelay);
            while (gameObject != null) {
                weightBehaviour.ShrinkAndCheck();
                yield return new WaitForSeconds(shrinkInterval);
            }
            decaying = false;
        }
    }

}
