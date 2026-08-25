using TMPro;
using UnityEngine;
using UnityEngine.UI;
using OMC;

public class XPBar : MonoBehaviour
{
    public Image fillImage;
    public TextMeshProUGUI text;

    public double level1Requirement = 100;
    public double levelUpRequirementMarkiplier = 1.2d;
    
    void Update()
    {
        double xp = GameLogicScript.score;

        int level = 0;
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
            text.text = $"level {level}";
        }
        if (fillImage)
        {
            fillImage.fillAmount = (float)(xpIn/xpReq);
        }
    }
}
