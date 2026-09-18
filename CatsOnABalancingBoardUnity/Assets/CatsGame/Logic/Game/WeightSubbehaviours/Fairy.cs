using System.Buffers;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Fairy : WeightSubBehaviourBase
{
    static readonly int emissionColorID = Shader.PropertyToID("_EmissionColor");
    static List<Fairy> instances = new();
    public static bool fairyExists => instances.Count > 0;
    Renderer renderer;
    Color glowColor;
    public float hueShift = 0.1f;

    public override void Start()
    {
        instances.Add(this);
        renderer = GetComponent<MeshRenderer>();
        glowColor = renderer.material.GetColor(emissionColorID);
        base.Start();
    }

    void OnDestroy()
    {
        instances.Remove(this);
    }

    public static void Respond()
    {
        foreach(var instance in instances)
        {
            instance.glowColor = ShiftHue(instance.glowColor,instance.hueShift);
            instance.renderer.material.SetColor(emissionColorID,instance.glowColor);
        }
    }

    public static Color ShiftHue(Color color, float shift)
    {
        Color.RGBToHSV(color, out float h, out float s, out float v);
        h = Mathf.Repeat(h + shift, 1);
        Color shifted = Color.HSVToRGB(h,s,v);
        shifted.a = color.a;
        return shifted;
    }
}
