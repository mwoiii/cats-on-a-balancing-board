using System.Linq;
using OMC.ECS;
using TMPro;
using UnityEngine;

public class MatchCatWeight : WeightSubBehaviourBase
{
    Rigidbody body;
    public TextMeshPro[] numbers;

    public override void Start()
    {
        base.Start();
        body = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        float mass = CatMassBridge.instance.mass;

        body.mass = mass * 2;
        string a = Mathf.RoundToInt(mass).ToString("D2");
        for (int i = 0; i < numbers.Length; i++)
        {
            numbers[i].text = a;
        }
    }
}
