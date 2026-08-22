using System.Text.RegularExpressions;
using OMC;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WeightTooltip : MonoBehaviour
{
    public static WeightTooltip instance;

    public GameObject obj;
    public Image image;
    public TextMeshProUGUI multAddText;
    public TextMeshProUGUI baseAddText;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descText;

    void Awake()
    {
        instance = this;
        Hide();
    }

    public void Show(WeightDef def, Vector2 pos)
    {
        obj.SetActive(true);

        image.sprite = def.sprite;

        multAddText.text = def.multAdd.ToString("+0.##;-0.##,;");

        baseAddText.text = def.baseAdd.ToString("+0.##;-0.##,;");

        string title = def.name.StartsWith("wd") ? def.name[2..] : def.name;
        titleText.text = Regex.Replace(title, "(?<!^)([A-Z])", " $1");

        descText.text = def.description;

        obj.transform.position = pos;
    }

    public void Hide()
    {
        obj.SetActive(false);
    }
}
