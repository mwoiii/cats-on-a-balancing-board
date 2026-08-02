#ifndef CAT_INSTANCE_COLOR_INCLUDED
#define CAT_INSTANCE_COLOR_INCLUDED

#ifdef UNITY_DOTS_INSTANCING_ENABLED
UNITY_DOTS_INSTANCING_START(UserPropertyMetadata)
    UNITY_DOTS_INSTANCED_PROP(float4, _BaseColor)
UNITY_DOTS_INSTANCING_END(UserPropertyMetadata)
#endif

void GetCatInstanceColor_float(out float4 Out)
{
#if defined(UNITY_DOTS_INSTANCING_ENABLED)
    Out = UNITY_ACCESS_DOTS_INSTANCED_PROP_WITH_DEFAULT(float4, _BaseColor);
#else
    Out = _BaseColor;
#endif
}

#endif