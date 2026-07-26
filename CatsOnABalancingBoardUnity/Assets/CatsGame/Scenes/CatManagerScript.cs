using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class CatManagerScript : MonoBehaviour
{
    public GameLogicScript logic;
    private List<GameObject> cats = new List<GameObject>();

    public AudioClip meow;
    public float minAmbientMeowInterval = 3;
    public float maxAmbientMeowInterval = 20;
    public float initialCatCount = 100;
    public float volume = 0.3f;
    public float minPitch = 0.5f;
    public float maxPitch = 1.5f;

    public static System.Action LostCat;

    public void RegisterCat(GameObject cat)
    {
        cats.Add(cat);
        if (HUDController.instance) { 
            HUDController.instance.UpdateRemainingCats(1); 
        }
        // Debug.Log("Cats count: " + cats.Count);
    }

    public void RemoveCat(GameObject cat)
    {
        cats.Remove(cat);
        if (HUDController.instance) { 
            HUDController.instance.UpdateRemainingCats(-1);
            LostCat.Invoke();
        }
        // Debug.Log("Cats count: " + cats.Count);
        if (cats.Count == 0)
        {
            logic.gameOver();
        }
    }

    public void ClearAllCats()
    {
        foreach (GameObject cat in cats)
        {
            if (cat != null)
                Destroy(cat);
        }

        cats.Clear();
    }

    public int GetCatCount()
    {
        return cats.Count;
    }

    void Start()
    {
        StartCoroutine(AmbientMeow());
    }

    IEnumerator AmbientMeow()
    {
        yield return new WaitForSeconds(Random.Range(minAmbientMeowInterval,maxAmbientMeowInterval));
        while (cats.Count > 0)
        {
            GameObject luckyWinner = cats[Random.Range(0,cats.Count)];
            AudioSource player = luckyWinner.AddComponent<AudioSource>();
            player.clip = meow;
            player.volume = volume;
            player.pitch = Random.Range(minPitch,maxPitch);
            player.spatialBlend = 1;
            player.Play();

            float populationCoeff = initialCatCount/cats.Count;
            yield return new WaitForSeconds(Random.Range(populationCoeff*minAmbientMeowInterval,populationCoeff*maxAmbientMeowInterval));
        }
    }
}
