using System.Collections.Generic;
using System.Linq;
using OMC;
using OMC.UI.CustomStyles;
using Unity.VisualScripting;
using UnityEngine;

public class BecomeOnContact : WeightSubBehaviourBase
{
    public AudioClip clip;
    public int charges {get; private set;} = 1;

    public float shakeRadius = 0.5f;
    public float slopeParam = 0.01f;

    public Transform disjointVisual;    

    static List<BecomeOnContact> instances = new();
    Unity.Mathematics.Random randomSauce = new(676767);
    float r = 0;

    public override void Start()
    {
        base.Start();
        instances.Add(this);
    }

    void OnDestroy()
    {
        instances.Remove(this);
    }

    void Update()
    {
        Vector3 offset = r * randomSauce.NextFloat3Direction();
        disjointVisual.position = transform.position + offset;
    }

    void OnCollisionEnter(Collision collision)
    {
        GameObject incoming = collision.collider.gameObject;
        if (incoming.TryGetComponent<WeightBehaviour>(out var a))
        {
            if (a.type == WeightBehaviour.WeightType.Antimatter) // unique antimatter interaction;
            {
                AddToCharges(charges);
            } 
            else if (a.type == weightBehaviour.type)
            {
                int incomingCharges = 1;
                if (incoming.TryGetComponent<BecomeOnContact>(out var b))
                {
                    incomingCharges = b.charges;
                }

                GameObject toDestroy = incomingCharges == charges ? instances.IndexOf(b) > instances.IndexOf(this) ? incoming : gameObject : incomingCharges < charges ? incoming : gameObject;

                Destroy(toDestroy);
                
                if (toDestroy != gameObject) 
                {
                    AddToCharges(incomingCharges);
                }
            }
            else
            {
                Transform temp = transform;
                Destroy(gameObject);
            
                GameObject butterfly = null;
                for (int i = 0; i < charges; i++)
                {
                    butterfly = Instantiate(collision.collider.gameObject);

                    butterfly.transform.localPosition = temp.localPosition;
                    butterfly.transform.rotation = temp.rotation;
                    butterfly.transform.localScale = temp.localScale;

                    WeightDropper.weightBehaviourDict[butterfly] = butterfly.GetComponent<WeightBehaviour>();
                }

                if (butterfly)
                {
                    if (!butterfly.TryGetComponent<AudioSource>(out var source))
                    {
                        source = butterfly.AddComponent<AudioSource>();
                    }
                    source.clip = clip;
                    source.Play(); 
                }
            }
        }
    }

    void AddToCharges(int x)
    {
        charges += x;
        r = shakeRadius * (1 - Mathf.Exp(-slopeParam * Mathf.Max(0,charges - 1)));
    }
}
