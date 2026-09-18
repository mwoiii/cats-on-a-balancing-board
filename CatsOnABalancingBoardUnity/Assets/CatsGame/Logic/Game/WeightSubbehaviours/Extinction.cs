using System.Linq;
using System.Net;
using OMC;
using Unity.VisualScripting;
using UnityEngine;

public class Extinction : WeightSubBehaviourBase
{
    bool hasLanded = false;
    public AudioSource source;
    public AudioClip clip;
    public float minPitch = 0.8f;
    public float maxPitch = 1.2f;

    public override void Start()
    {
        base.Start();
        if (source && clip)
        {
            source.clip = clip;
            source.pitch = Random.Range(minPitch,maxPitch);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.CompareTag("Board"))
        {
            hasLanded = true;
        }
    }

    void OnDestroy()
    {
        if (!hasLanded)
        {
            return;
        }
        
        var a = WeightDropper.weightBehaviourDict.Keys.ToList();
        if (a.Count > 0)
        {
            GameObject b = a[UnityEngine.Random.Range(0,a.Count)];
            Transform temp = b.transform;
            Destroy(b);
            GameObject c = Instantiate(gameObject);
            c.transform.SetPositionAndRotation(temp.position, temp.rotation);
            c.SetActive(true);
            var d = c.GetComponent<AudioSource>();
            d.enabled = true;
            d.Play();
        }
    }
}
