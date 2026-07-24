using UnityEngine;
using System.Collections;

public class CatPopulater : MonoBehaviour
{
    public GameObject catPrefab;
    public int catCount = 10;
    public float dropHeight = 0.5f;
    public int batchSize = 5;
    public float batchInterval = 0.1f;
    public float edgeMargin = 0.5f;


    float halfX,halfZ,spawnY;
    Vector3 boardCenter;
    void Start()
    {
        GameObject board = GameObject.FindGameObjectWithTag("Board");
        Bounds bounds = board.GetComponent<Renderer>().bounds;

        halfX = bounds.extents.x - edgeMargin;
        halfZ = bounds.extents.z - edgeMargin;
        spawnY = bounds.max.y + dropHeight;
        boardCenter = bounds.center;

        StartCoroutine(Populate());
    }

    void Update()
    {
        
    }

    IEnumerator Populate()
    {
        int spawned = 0;
        while (spawned < catCount)
        {
            int batchCount = Mathf.Min(batchSize,catCount-spawned);
            for (int i = 0; i < batchCount; i++)
            {
                Vector2 pos = RandomPointInEllipse(halfX,halfZ);
                Vector3 spawnPos = new Vector3(boardCenter.x + pos.x, spawnY, boardCenter.z + pos.y);
                Instantiate(catPrefab,spawnPos,Quaternion.identity);
            }
            spawned += batchCount;
            
            yield return new WaitForSeconds(batchInterval);
        }
        
    }

    Vector2 RandomPointInEllipse(float halfX,float halfZ)
    {
        Vector2 point;
        do
        {
            point = new Vector2(Random.Range(-halfX,halfX),Random.Range(-halfZ,halfZ));
        }
        while ((point.x * point.x) / (halfX * halfX) + (point.y * point.y) / (halfZ * halfZ) > 1);

        return point;
    }
}
