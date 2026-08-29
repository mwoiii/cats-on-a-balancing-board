using System.Collections;
using Assets.CatsGame.Logic.Game;
using OMC;
using UnityEditor.PackageManager;
using UnityEngine;

public class ChangeTimeOnCombine : WeightSubBehaviourBase
{
    public float timescale = 1;
    public float realTimeDuration = 5;

    public AudioClip startSound;
    public AudioClip endSound;
    
    public static AudioSource source;
    public static CoroutineHost host;
    static Coroutine active;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.TryGetComponent<WeightBehaviour>(out var a) && a.type == weightBehaviour.type)
        {
            if (!source || !host){SetupMothership();}
            if (active != null)
            {
                host.StopCoroutine(active);
            }
            active = host.StartCoroutine(ChangeTimescale(startSound,endSound,timescale,realTimeDuration));
            Destroy(gameObject);
        }
    }

    static IEnumerator ChangeTimescale(AudioClip startSound, AudioClip endSound, float timescale, float realTimeDuration)
    {
        Time.timeScale = GlobalTimescale.timeScale * timescale;
        source.clip = startSound;
        source.Play();

        yield return new WaitForSeconds(realTimeDuration*timescale);

        Time.timeScale = GlobalTimescale.timeScale;
        source.clip = endSound;
        source.Play();
    }

    static void SetupMothership()
    {
        GameObject a = new("ChangeTimeOnCombine Mothership");
        source = a.AddComponent<AudioSource>();
        host = a.AddComponent<CoroutineHost>();
    }
}

public class CoroutineHost : MonoBehaviour {}
