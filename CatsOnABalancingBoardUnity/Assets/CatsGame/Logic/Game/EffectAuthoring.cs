using Assets.CatsGame.Logic.Game;
using System;
using Unity.Entities;
using UnityEngine;

public class EffectAuthoring : MonoBehaviour {
    public float lifetime = 0.8f;

    [NonSerialized]
    public EffectType type = EffectType.Misc;

    class Baker : Baker<EffectAuthoring> {
        public override void Bake(EffectAuthoring authoring) {
            var entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new EffectData { lifetime = authoring.lifetime, type = authoring.type });
        }
    }
}
