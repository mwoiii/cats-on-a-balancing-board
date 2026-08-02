using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using static StackingGridSystem;

public class StackingMatrixAuthoring : MonoBehaviour {
    class Baker : Baker<StackingMatrixAuthoring> {
        public override void Bake(StackingMatrixAuthoring authoring) {
            var entity = GetEntity(TransformUsageFlags.None);
            var buffer = AddBuffer<StackingMatrixData>(entity);
            buffer.Resize(width * height, NativeArrayOptions.ClearMemory);
        }
    }
}

public struct StackingMatrixData : IBufferElementData {
    public ushort value;
}
