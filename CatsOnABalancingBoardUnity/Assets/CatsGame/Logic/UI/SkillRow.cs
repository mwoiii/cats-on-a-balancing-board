using OMC;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillRow : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI pointsText;
    public Button minusButton;
    public Button plusButton;

    SkillDef def;
    SkillPanelController controller;

    public void Setup(SkillDef def, SkillPanelController controller)
    {
        this.def = def;
        this.controller = controller;

        nameText.text = def.skillName;

        plusButton.onClick.AddListener(()=> controller.TryAddPoint(def));
        minusButton.onClick.AddListener(()=> controller.TryRemovePoint(def));

        Refresh();
    }

    public void Refresh()
    {
        pointsText.text = controller.GetPoints(def).ToString();
    }
}
