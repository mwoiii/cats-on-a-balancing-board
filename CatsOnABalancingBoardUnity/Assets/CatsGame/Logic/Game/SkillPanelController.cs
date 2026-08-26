using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using OMC;
using OMC.ECS;
using TMPro;
using UnityEngine;

public class SkillPanelController : MonoBehaviour
{
    public List<SkillDef> skillDefs = new();
    public SkillRow rowPrefab;
    public Transform rowContainer;
    public TextMeshProUGUI remainingPointsText;

    Dictionary<SkillDef, int> spread = new();
    List<SkillRow> rows = new();

    void OnEnable()
    {
        foreach(Transform child in rowContainer)
        {
            Destroy(child.gameObject);
        }
        rows.Clear();

        foreach(var def in skillDefs)
        {
            if (!spread.ContainsKey(def))
            {
                spread[def] = 0;
            }

            SkillRow row = Instantiate(rowPrefab,rowContainer);
            row.Setup(def,this);
            rows.Add(row);

            RefreshAll();
        }
    }

    void OnDisable()
    {
        ApplySkills();
    }

    public int GetPoints(SkillDef def)
    {
        return spread.TryGetValue(def, out int value) ? value : 0;
    }

    int TotalAllocated()
    {
        int total = 0;
        foreach (var a in spread)
        {
            total += a.Value;
        }
        return total;
    }

    int PointsAvailable()
    {
        return XPBar.instance? XPBar.instance.level : 0;
    }

    int PointsRemaining()
    {
        return PointsAvailable() - TotalAllocated();
    }

    public void TryAddPoint(SkillDef def)
    {
        if (PointsRemaining() <= 0){return;}
        if (def.maxPoints > 0 && GetPoints(def) >= def.maxPoints){return;}

        spread[def]++;
        RefreshAll();
    }

    public void TryRemovePoint(SkillDef def)
    {
        if (GetPoints(def) <= 0){return;}

        spread[def]--;
        RefreshAll();
    }

    void RefreshAll()
    {
        foreach (var row in rows)
        {
            row.Refresh();
        }
        if (remainingPointsText)
        {
            remainingPointsText.text = $"points: {PointsRemaining()}";
        }
    }

    void ApplySkills()
    {
        foreach(var def in skillDefs)
        {
            switch (def.name)
            {
                case "sdBoardSize":
                    BoardController.instance.ChangeRadius(3 + 0.5f*spread[def]);
                    break;
                case"sdLighterCats":
                    CatMassBridge.instance.lighterCatsPoints = spread[def];
                    break;
                case "sdShorterTricks":
                    TrickTimer.instance.shorterTricksPoints = spread[def];
                    break;
            }
        }
    }
}
