using System.Collections.Generic;
using UnityEngine;

public class CatManagerScript : MonoBehaviour
{
    public GameLogicScript logic;
    private List<GameObject> cats = new List<GameObject>();
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void RegisterCat(GameObject cat)
    {
        cats.Add(cat);
        // Debug.Log("Cats count: " + cats.Count);
    }

    public void RemoveCat(GameObject cat)
    {
        cats.Remove(cat);
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
}
