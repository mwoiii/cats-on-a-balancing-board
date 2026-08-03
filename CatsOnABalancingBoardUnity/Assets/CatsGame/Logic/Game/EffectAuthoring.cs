using Assets.CatsGame.Logic.Game;
using Unity.Entities;
using UnityEngine;

public class EffectAuthoring : MonoBehaviour {
    public float lifetime = 0.8f;

    class Baker : Baker<EffectAuthoring> {
        public override void Bake(EffectAuthoring authoring) {
            var entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new EffectData { lifetime = authoring.lifetime });
        }
    }
}
