void BillboardWorldPosition_float(float3 localPos, float size, out float3 WorldPosition)
{
    float3 pivot = TransformObjectToWorld(float3(0,0,0));

    float3 camRight = UNITY_MATRIX_I_V._m00_m10_m20;
    float3 camUp = UNITY_MATRIX_I_V._m01_m11_m21;

    WorldPosition = pivot + camRight * localPos.x * size + camUp * localPos.y * size;
}
