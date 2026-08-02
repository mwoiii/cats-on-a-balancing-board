using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using static StackingGridSystem;

public class StackingGridAuthoring : MonoBehaviour {
    class Baker : Baker<StackingGridAuthoring> {
        public override void Bake(StackingGridAuthoring authoring) {
            var entity = GetEntity(TransformUsageFlags.None);
            var buffer = AddBuffer<StackingGridData>(entity);
            buffer.Resize(width * height, NativeArrayOptions.ClearMemory);
        }
    }
}

public struct StackingGridData : IBufferElementData {
    public ushort value;
}
