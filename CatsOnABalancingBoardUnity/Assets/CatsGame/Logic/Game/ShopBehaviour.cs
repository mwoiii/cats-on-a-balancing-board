using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Assets.CatsGame.Logic.Game;
using OMC;
using OMC.UI;
using Unity.Mathematics;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.UI;

public class ShopBehaviour : MonoBehaviour
{
    public static ShopBehaviour instance;

    const int choiceCount = 3;

    public GameObject masterObject;
    
    public GameObject[] choiceSlotObjects = new GameObject[choiceCount];
    public WeightPreviewSpinner[] spinners = new WeightPreviewSpinner[choiceCount];
    WeightDef[] choices;
    
    public Sprite kikiAura;
    public Sprite boboAura;
    public Sprite fufuAura;
    public Color baseColor;
    public Color multColor;
    public Color fufuColor;

    public float cooldownDuration = 60;
    float cooldownEnd = -1;

    void Start()
    {
        instance = this;
    }

    public void OpenShop()
    {
        if (Time.unscaledTime < cooldownEnd)
        {
            Debug.Log("Shop refused, on cooldown"); 
            return;
        }

        masterObject.SetActive(true);
        rotationSelectPanel.SetActive(false);

        Time.timeScale = 0;

        SetupChoices();
    }

    public void CloseShop()
    {
        masterObject.SetActive(false);

        Time.timeScale = GlobalTimescale.timeScale;

        cooldownEnd = Time.unscaledTime + cooldownDuration;
    }

    void SetupChoices()
    {
        choices = WeightTypeRegistry.GetRandomWeightDefs(choiceCount, WeightDropper.instance.GetCurrentRotation());
        
        for(int i = 0; i < choiceCount; i++)
        {
            SetAura(choiceSlotObjects[i].GetComponent<Image>(), choices[i]);

            spinners[i].TrySetPreview(choices[i].shapePrefabs[0]);
        }
    }

    void SetAura(Image image, WeightDef def)
    {
        Debug.Log($"{def.name} ::: mult {def.multAdd} ::: base {def.baseAdd}");
        float multSign = math.sign(def.multAdd);
        float baseSign = math.sign(def.baseAdd);
        float signDiff = math.abs(multSign - baseSign);
        
        if (signDiff == 2)
        {
            Debug.LogError($"{def.name} is both kiki and bobo");
        }
        else if (multSign == 0 && baseSign == 0)
        {
            image.sprite = fufuAura;
            image.color = fufuColor;
        }
        else
        {
            image.sprite = multSign + baseSign > 0 ? kikiAura : boboAura;
            image.color = baseSign != 0 ? baseColor : multColor; // in a def with contribution to base&mult, the base color appears (this could be changed later)
        } 
    }

    public GameObject rotationSelectPanel;
    public GameObject rotationButtonPrefab;

    WeightDef pendingIncoming;

    public void SelectChoice(int index)
    {
        pendingIncoming = choices[index];
        OpenRotationSelect();
    }

    void OpenRotationSelect()
    {
        foreach (Transform child in rotationSelectPanel.transform)
        {
            Destroy(child.gameObject);
        }

        List<WeightDef> rotatoin = WeightDropper.instance.GetCurrentRotation();
        foreach (WeightDef def in rotatoin)
        {
            Debug.Log($"{def.name} ::: mult {def.multAdd} ::: base {def.baseAdd}");

            GameObject button = Instantiate(rotationButtonPrefab,rotationSelectPanel.transform);
            button.GetComponent<Image>().sprite = def.sprite;

            button.GetComponent<Button>().onClick.AddListener(() => OnRotationSlotSelected(def));
        }

        rotationSelectPanel.SetActive(true);
    }

    void OnRotationSlotSelected(WeightDef outgoing)
    {
        WeightDropper.instance.SubstituteInRotation(outgoing, pendingIncoming);
        CloseShop();
    }
}
