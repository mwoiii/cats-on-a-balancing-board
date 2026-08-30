using System;
using System.Xml.XPath;
using OMC;
using UnityEngine;

public class XPOnDestroy : WeightSubBehaviourBase
{
    XPBar xpBar;
    public float levelPortionToGive = 0.2f;

    bool can = true;
    public override void Start()
    {
        base.Start();
        xpBar = XPBar.instance;
        if (TryGetComponent<DecayOverTime>(out var a))
        {
            a.DecayStart += () => {can = false;};
        }
    }

    void OnDestroy()
    {
        if (xpBar && can)
        {
            double toGive = levelPortionToGive * Math.Max(xpBar.level1Requirement * xpBar.level * xpBar.levelUpRequirementMarkiplier,xpBar.level1Requirement);
            GameLogicScript.instance.AddToScore(toGive);
        }
    }
}
