using System;
using System.Collections.Generic;
using System.Xml;
using OMC;
using Unity.VisualScripting;
using UnityEngine;

public class WeightTypeRegistry : MonoBehaviour
{
    public static WeightTypeRegistry instance {get; private set;}

    public TextAsset weightTypeConfig;

    [SerializeField] List<WeightPrefabSlot> weightPrefabSlots = new();

    public List<WeightTypeData> weightTypes {get; private set;}
    
    Dictionary<WeightBehaviour.WeightType, WeightTypeData> byType = new();

    void Awake()
    {
        instance = this;
        weightTypes = new();
        Load();
    }

    void Load()
    {
        weightTypes.Clear();
        byType.Clear();

        if (weightTypeConfig == null)
        {
            Debug.LogError("Waiter! one csv please");
            return;
        }

        List<WeightTypeConfigRow> rows = WeightTypeConfigCSV.Parse(weightTypeConfig.text);

        foreach (var row in rows)
        {
            if (!Enum.TryParse(row.typeName, true, out WeightBehaviour.WeightType a))
            {
                Debug.LogError($"need matching WeightType for csv typeName {row.typeName}");
                continue;
            }

            List<GameObject> prefabs = new();
            foreach (string shape in row.shapes)
            {
                WeightPrefabSlot slot = weightPrefabSlots.Find(s => s.typeName == row.typeName && s.shapeName == shape);
                if (slot.prefab == null)
                {
                    Debug.LogWarning($"no prefab for {row.typeName} {shape}");
                    continue;
                }

                if (!slot.prefab.TryGetComponent<WeightBehaviour>(out var beh)) {beh = slot.prefab.AddComponent<WeightBehaviour>();}
                beh.type = a;
                if (!slot.prefab.TryGetComponent<Rigidbody>(out var rig)) {rig = slot.prefab.AddComponent<Rigidbody>();}
                rig.mass = Mathf.Max(row.weight, 1e-7f);
                
                prefabs.Add(slot.prefab);
            }

            if (prefabs.Count == 0) { continue; }

            WeightTypeData data = new()
            {
                typeName = row.typeName,
                type = a,
                mass = Mathf.Max(row.weight, 1e-7f),
                probabilityBias = row.probabilityBias,
                rarity = row.rarity,
                multAdd = row.multAdd,
                baseAdd = row.baseAdd,
                shapePrefabs = prefabs.ToArray()
            };

            weightTypes.Add(data);
            byType[a] = data;
        }

        if (weightTypes.Count == 0)
        {
            Debug.LogError("HOW");
        }
    }
    public GameObject GetRandomShapePrefab(WeightTypeData data)
    {
        return data.shapePrefabs[UnityEngine.Random.Range(0, data.shapePrefabs.Length)];
    }

    public WeightTypeData GetRandomWeightedType(List<WeightTypeData> selection)
    {
        if (selection.Count == 0) { return null; }

        float total = 0f;
        foreach (var t in selection) { total += t.probabilityBias; }

        float roll = UnityEngine.Random.Range(0, total);
        float cum = 0f;

        foreach (var t in selection)
        {
            cum += t.probabilityBias;
            if (roll < cum) { return t; }
        }

        return selection[^1];
    }
}

