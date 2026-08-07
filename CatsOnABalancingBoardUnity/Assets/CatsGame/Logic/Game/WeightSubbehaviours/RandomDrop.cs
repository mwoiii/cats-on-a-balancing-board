using UnityEngine;

public class RandomDrop : WeightSubbehaviour
{
    new void Start()
    {
        base.Start();
        
        float boardRadius = GameObject.FindGameObjectWithTag("Board").GetComponent<Collider>().bounds.extents.x;
        Vector2 b = UnityEngine.Random.insideUnitCircle * boardRadius;
        transform.position = new Vector3(b.x, 3, b.y);
    }
}
