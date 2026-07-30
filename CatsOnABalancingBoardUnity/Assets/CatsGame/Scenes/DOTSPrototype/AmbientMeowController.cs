using System.Collections;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class AmbientMeowController : MonoBehaviour
{
    public AudioClip meow;
    public float minAmbientMeowInterval = 3;
    public float maxAmbientMeowInterval = 20;
    public float assumedBasePopulation = 100;
    public float volume = 0.3f;
    public float minPitch = 0.5f;
    public float maxPitch = 1.5f;

    EntityManager boss;
    EntityQuery query;

    void Start()
    {
        boss = World.DefaultGameObjectInjectionWorld.EntityManager;
        query = boss.CreateEntityQuery(typeof(CatData), typeof(LocalTransform));
        StartCoroutine(AmbientMeow());
    }

    IEnumerator AmbientMeow()
    {
        yield return new WaitForSeconds(UnityEngine.Random.Range(minAmbientMeowInterval, maxAmbientMeowInterval));
        int catCount = query.CalculateEntityCount();

        while (catCount > 0)
        {
            NativeArray<LocalTransform> transforms = query.ToComponentDataArray<LocalTransform>(Allocator.Temp);
            float3 pos = transforms[UnityEngine.Random.Range(0,transforms.Length)].Position;
            transforms.Dispose();

            GameObject audioObject = new("Uhhh. Meow");
            audioObject.transform.position = new Vector3(pos.x,pos.y,pos.z);
            AudioSource sauce = audioObject.AddComponent<AudioSource>();
            sauce.clip = meow;
            sauce.volume = volume;
            sauce.pitch = UnityEngine.Random.Range(minPitch,maxPitch);
            sauce.spatialBlend = 1;
            sauce.Play();
            Destroy(audioObject, meow != null ? meow.length : 1f);

            float populationCoeff = Mathf.Sqrt(Mathf.Sqrt(assumedBasePopulation / catCount));
            //populationCoeff = Mathf.Min(0.1f, populationCoeff); // optional ceiling for meow frequency. kind of lame idk
            yield return new WaitForSeconds(UnityEngine.Random.Range(populationCoeff * minAmbientMeowInterval, populationCoeff * maxAmbientMeowInterval));

            catCount = query.CalculateEntityCount();
        }
    }
}