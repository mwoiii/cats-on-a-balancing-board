using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using static StackingGridSystem;

public class StackingGridAuthoring : MonoBehaviour {
    class Baker : Baker<StackingGridAuthoring> {
        public override void Bake(StackingGridAuthoring authoring) {
            var entity = GetEntity(TransformUsageFlags.None);
            var stackingBuffer = AddBuffer<StackingGridData>(entity);
            stackingBuffer.Resize(width * height, NativeArrayOptions.ClearMemory);
            var refreshBuffer = AddBuffer<RefreshGridData>(entity);
            refreshBuffer.Resize(width * height, NativeArrayOptions.ClearMemory);
            AddComponent<RefreshGridValue>(entity);
        }
    }
}

public struct StackingGridData : IBufferElementData {
    public ushort value;
}

public struct RefreshGridData : IBufferElementData {
    public byte value;
}

public struct RefreshGridValue : IComponentData {
    public byte value;
}
