using System.Collections.Generic;
using OMC;
using Unity.Entities;
using Unity.VectorGraphics;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;

public class Jester : WeightSubBehaviourBase
{
    public int shapeIndex = 0;

    public List<WeightDef> exclude;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.CompareTag("Board"))
        {
            WeightDef identity;
            GameObject prefab = null;
            while (prefab == null)
            {
                identity = WeightTypeRegistry.GetRandomWeightDefs(1,exclude)[0]; // mhm
                switch (shapeIndex)
                {
                    case 0:
                        prefab = identity.blockPrefab;
                        break;
                    case 1:
                        prefab = identity.spherePrefab;
                        break;
                    default:
                        prefab = identity.GetRandomShapePrefab();
                        break;
                }
            }
            collision.thisCollider.enabled = false;
            Destroy(gameObject);
            var tilcayo = Instantiate(prefab);
            tilcayo.transform.SetPositionAndRotation(transform.position, transform.rotation);
            WeightDropper.weightBehaviourDict[tilcayo] = tilcayo.GetComponent<WeightBehaviour>();
        }
    }
}
