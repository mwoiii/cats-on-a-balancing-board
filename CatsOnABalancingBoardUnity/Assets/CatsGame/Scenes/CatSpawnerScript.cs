using System.Collections;
using UnityEngine;

public class CatSpawnerScript : MonoBehaviour
{
    public GameObject catPrefab;
    public CatManagerScript catManager;
    
    public int startingCatCount = 10;
    public float dropHeight = 0.5f;
    public int batchSize = 5;
    public float batchInterval = 0.1f;
    public float edgeMargin = 0.5f;
    
    float halfX, halfZ, spawnY;
    private Vector3 boardCenter;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject board = GameObject.FindGameObjectWithTag("Board");
        Bounds bounds = board.GetComponent<Renderer>().bounds;

        halfX = bounds.extents.x - edgeMargin;
        halfZ = bounds.extents.z - edgeMargin;
        spawnY = bounds.max.y + dropHeight;
        boardCenter = bounds.center;

        StartCoroutine(PopulateBoard());
    }

    IEnumerator PopulateBoard()
    {
        int spawned = 0;
        while (spawned < startingCatCount)
        {
            int batchCount = Mathf.Min(batchSize,startingCatCount-spawned);
            for (int i = 0; i < batchCount; i++)
            {
                Vector2 boardPos = RandomBoardPosition();
                Vector3 spawnPos = new Vector3(boardPos.x, spawnY, boardPos.y);
                
                GameObject cat = Instantiate(catPrefab, spawnPos, Quaternion.identity);

                if (catManager != null)
                {
                    catManager.RegisterCat(cat);
                }
            }
            spawned += batchCount;
            
            yield return new WaitForSeconds(batchInterval);
        }
        
    }

    Vector2 RandomBoardPosition()
    {
        Vector2 boardOffset;
        do
        {
            boardOffset = new Vector2(Random.Range(-halfX,halfX),Random.Range(-halfZ,halfZ));
        }
        while ((boardOffset.x * boardOffset.x) / (halfX * halfX) + (boardOffset.y * boardOffset.y) / (halfZ * halfZ) > 1);
        Vector2 boardPosition = new Vector2(boardCenter.x + boardOffset.x, boardCenter.z + boardOffset.y);
        return boardPosition;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
