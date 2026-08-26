using TMPro;
using UnityEngine;
using UnityEngine.UI;
using OMC;

public class XPBar : MonoBehaviour
{
    public static XPBar instance;
    public int level {get; private set;} = 0;

    public Image fillImage;
    public TextMeshProUGUI text;

    public double level1Requirement = 100;
    public double levelUpRequirementMarkiplier = 1.2d;
    public double initialXP = 0;

    void Start()
    {
        instance = this;

        if (text)
        {
            text.text = $"lvl.0";
        }
        if (fillImage)
        {
            fillImage.fillAmount = 0;
        }
    }

    void Update()
    {
        double xp = initialXP + GameLogicScript.score;

        level = 0;
        double xpIn = xp;
        double xpReq = level1Requirement;

        while (xpIn >= xpReq)
        {
            xpIn -= xpReq;
            xpReq *= levelUpRequirementMarkiplier;
            level++;
        }

        if (text)
        {
            text.text = $"lvl.{level}";
        }
        if (fillImage)
        {
            fillImage.fillAmount = (float)(xpIn/xpReq);
        }
    }
}
