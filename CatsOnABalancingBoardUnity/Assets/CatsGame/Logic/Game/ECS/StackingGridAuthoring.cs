using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using static OMC.ECS.StackingGridSystem;

namespace OMC.ECS {
    public class StackingGridAuthoring : MonoBehaviour {
        class Baker : Baker<StackingGridAuthoring> {
            public override void Bake(StackingGridAuthoring authoring) {
                var entity = GetEntity(TransformUsageFlags.None);
                var stackingBuffer = AddBuffer<StackingGridData>(entity);
                stackingBuffer.Resize(Width * Height, NativeArrayOptions.ClearMemory);
                var refreshBuffer = AddBuffer<RefreshGridData>(entity);
                refreshBuffer.Resize(Width * Height, NativeArrayOptions.ClearMemory);
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
}
