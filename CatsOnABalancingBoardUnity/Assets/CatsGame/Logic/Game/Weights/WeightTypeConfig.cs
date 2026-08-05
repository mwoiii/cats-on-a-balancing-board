using System;
using System.Collections.Generic;
using System.Globalization;
using OMC;
using UnityEngine;

[System.Serializable]
public struct WeightTypeConfigRow
{
    public string typeName;
    public float weight;
    public float probabilityBias;
    public float rarity;
    public float multAdd;
    public float baseAdd;
    public List<string> shapes;
}

[System.Serializable]
public struct WeightPrefabSlot
{
    public string typeName;
    public string shapeName;
    public GameObject prefab;
}

public class WeightTypeData
{
    public string typeName;
    public OMC.WeightBehaviour.WeightType type;
    public float mass;
    public float probabilityBias;
    public float rarity;
    public float multAdd;
    public float baseAdd;
    public GameObject[] shapePrefabs;
}

public static class WeightTypeConfigCSV
{
    public static List<WeightTypeConfigRow> Parse(string text)
    {
        List<WeightTypeConfigRow> rows = new();
        if (string.IsNullOrWhiteSpace(text)){return rows;}

        string[] lines = text.Replace("\r\n","\n").Replace("\r","\n").Split("\n");
        if (lines.Length == 0){return rows;}

        string[] headers = SplitLine(lines[0]);
        int typeNameCol = Array.IndexOf(headers,"typeName");
        int weightCol = Array.IndexOf(headers,"weight");
        int biasCol = Array.IndexOf(headers,"probabilityBias");
        int rarityCol = Array.IndexOf(headers,"rarity");
        int multAddCol = Array.IndexOf(headers,"multAdd");
        int baseAddCol = Array.IndexOf(headers,"baseAdd");

        if (typeNameCol < 0 || biasCol < 0)
        {
            Debug.LogError("Nah! (bad csv file)");
            return rows;
        }
        
        List<(int col,string shapeName)> shapeCols = new();
        for (int i = 0; i < headers.Length; i++)
        {
            string h = headers[i].Trim();
            if (h.StartsWith("can") && h.Length > 3)
            {
                shapeCols.Add((i,h[3..]));
            }
        }

        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])){continue;}
            
            string[] cells = SplitLine(lines[i]); // Interlinked
            if (cells.Length <= typeNameCol || cells.Length <= biasCol){continue;}

            WeightTypeConfigRow row = new()
            {
                typeName = cells[typeNameCol].Trim(),
                weight = SmartParse(cells, weightCol, 1f),
                probabilityBias = float.Parse(cells[biasCol].Trim(), CultureInfo.InvariantCulture),
                rarity = SmartParse(cells, rarityCol, 0f),
                multAdd = SmartParse(cells, multAddCol, 0f),
                baseAdd = SmartParse(cells, baseAddCol, 0f),
                shapes = new List<string>()
            };

            foreach (var (col, shape) in shapeCols)
            {
                if (col < cells.Length && bool.TryParse(cells[col].Trim(), out bool can) && can)
                {
                    row.shapes.Add(shape);
                }
            }
            rows.Add(row);
        }
        return rows;
    }

    static string[] SplitLine(string line)
    {
        string[] raw = line.Split(',');
        for (int i = 0; i < raw.Length; i++){raw[i] = raw[i].Trim();}
        return raw;
    }

    static float SmartParse(string[] cells, int col, float backup)
    {
        if (col >= 0 && col < cells.Length && float.TryParse(cells[col].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out float value)){return value;}
        return backup;
    }
}
