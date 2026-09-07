using System.Linq;
using OMC.ECS;
using TMPro;
using UnityEngine;

public class MatchCatMass : WeightSubBehaviourBase
{
    public float relativeMassFactor = 1;
    public float minimumMass = 10;
    public TextMeshPro[] numbers;
    Rigidbody body;
    CatMassBridge catMassBridge;

    public override void Start()
    {
        base.Start();
        
        body = GetComponent<Rigidbody>();
        catMassBridge = CatMassBridge.instance;
    }

    void FixedUpdate()
    {
        body.mass = Mathf.Max(minimumMass, catMassBridge.mass * relativeMassFactor);
        
        string a = Mathf.RoundToInt(catMassBridge.mass).ToString("D2");
        for (int i = 0; i < numbers.Length; i++)
        {
            numbers[i].text = a;
        }
    }
}
