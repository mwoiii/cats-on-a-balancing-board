using System;
using OMC;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WeightTooltipHoverTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public WeightDef def;

    public event Action<WeightDef> OnHoverEnter;
    public event Action<WeightDef> OnHoverExit;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (def)
        {
            WeightTooltip.instance.Show(def,eventData.position);
            OnHoverEnter?.Invoke(def);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        WeightTooltip.instance.Hide();
        OnHoverExit?.Invoke(def);
    }
}
