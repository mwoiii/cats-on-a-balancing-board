using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Reflection;
using Assets.CatsGame.Logic.Game;
using OMC;
using OMC.UI;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ShopBehaviour : MonoBehaviour
{
    public static ShopBehaviour instance;

    const int choiceCount = 3;

    public GameObject masterObject;
    public GameObject weightExchangeObject;
    public GameObject skillPointSpecObject;
    
    public GameObject[] choiceSlotObjects = new GameObject[choiceCount];
    public WeightPreviewSpinner[] spinners = new WeightPreviewSpinner[choiceCount];
    WeightDef[] choices;

    public GameObject shopPrompt;

    public TextMeshProUGUI formulaText;
    
    public Sprite kikiAura;
    public Sprite boboAura;
    public Sprite fufuAura;
    public Color baseColor;
    public Color multColor;
    public Color fufuColor;

    public static event System.Action OnShopClosed;

    void Start()
    {
        instance = this;

        //ActivateShopPrompt();
    }

    void Update()
    {
        if (Keyboard.current.oKey.wasPressedThisFrame)
        {
            OpenShop();
        }
    }

    public void ActivateShopPrompt()
    {
        shopPrompt.SetActive(true);
    }

    void OpenShop()
    {
        if (!shopPrompt.activeSelf)
        {
            Debug.Log("shop prompt not active");
            return;
        }
        shopPrompt.SetActive(false);

        masterObject.SetActive(true);
        weightExchangeObject.SetActive(true);
        rotationSelectPanel.SetActive(false);
        skillPointSpecObject.SetActive(false);

        ShowCurrentFormula();

        Time.timeScale = 0;

        SetupChoices();
    }

    public void GoToSkillSpecScreen()
    {
        weightExchangeObject.SetActive(false);
        skillPointSpecObject.SetActive(true);
    }

    public void CloseShop()
    {
        masterObject.SetActive(false);

        Time.timeScale = GlobalTimescale.timeScale;

        OnShopClosed?.Invoke();
    }

    void SetupChoices()
    {
        choices = WeightTypeRegistry.GetRandomWeightDefs(choiceCount, WeightDropper.instance.GetCurrentRotation());
        
        for(int i = 0; i < choiceCount; i++)
        {
            SetAura(choiceSlotObjects[i].GetComponent<Image>(), choices[i]);

            spinners[i].TrySetPreview(choices[i].shapePrefabs[0]);

            choiceSlotObjects[i].GetComponent<WeightTooltipHoverTrigger>().def = choices[i];
        }
    }

    void SetAura(Image image, WeightDef def)
    {
        Debug.Log($"{def.name} ::: mult {def.multAdd} ::: base {def.baseAdd}");
        
        float sum = def.multAdd + def.baseAdd;
        if (!(sum == def.multAdd || sum == def.baseAdd))
        {
            Debug.LogWarning("no aura logic for type with both mult and base contributions");
            image.color = Color.clear;
        }
        else if (def.multAdd == 10 || def.baseAdd == 0.1)
        {
            image.sprite = fufuAura;
            image.color = fufuColor;
        }
        else
        {
            image.sprite = def.multAdd < 10 || def.baseAdd < 0.1 ? boboAura : kikiAura;
            image.color = def.multAdd > 0 || def.baseAdd > 0 ? multColor : baseColor;
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

            var trigger = button.GetComponent<WeightTooltipHoverTrigger>();
            trigger.def = def;
            trigger.OnHoverEnter += PreviewFormula;
            trigger.OnHoverExit += ShowCurrentFormula;

            button.GetComponent<Button>().onClick.AddListener(() => OnRotationSlotSelected(def));
        }

        rotationSelectPanel.SetActive(true);
    }

    void OnRotationSlotSelected(WeightDef outgoing)
    {
        WeightDropper.instance.SubstituteInRotation(outgoing, pendingIncoming);
        WeightTooltip.instance.Hide();
        GoToSkillSpecScreen();
    }

    void ShowCurrentFormula(WeightDef unused = null)
    {
        var (multVal, baseVal) = WeightDropper.ComputeBonusFormula();
        formulaText.text = FormatFormula(multVal, baseVal);
    }

    void PreviewFormula(WeightDef outgoing)
    {
        var (multVal, baseVal) = WeightDropper.ComputeBonusFormula(outgoing, pendingIncoming);
        formulaText.text = FormatFormula(multVal, baseVal);
    }

    string FormatFormula(float multVal, float baseVal)
    {
        string multHex = UnityEngine.ColorUtility.ToHtmlStringRGB(multColor);
        string baseHex = UnityEngine.ColorUtility.ToHtmlStringRGB(baseColor);
        return $"<color=#{multHex}>{multVal}</color> * <color=#{baseHex}>{baseVal}</color><sup><i>COMBO</i></sup>";
    }
}
