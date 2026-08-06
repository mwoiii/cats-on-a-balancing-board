using UnityEngine;
using System.Collections;
using OMC;

public class DecayOverTime : WeightSubbehaviour
{
    private float shrinkInterval = 0.8f;

    void OnCollisionEnter(Collision collision)
    {
        StartCoroutine(Decay());
    }

    IEnumerator Decay() {
    while (gameObject != null) {
        weightBehaviour.ShrinkAndCheck();
        yield return new WaitForSeconds(shrinkInterval);
        }
    }

}
