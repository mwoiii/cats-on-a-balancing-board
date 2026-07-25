using System.Collections.Generic;
using UnityEngine;

public class CatManagerScript : MonoBehaviour
{
    public GameLogicScript logic;
    private List<GameObject> cats = new List<GameObject>();

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
}
