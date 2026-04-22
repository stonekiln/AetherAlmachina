using System;
using System.IO;
using AetherAlmachina.Skill;
using DConfig.ActPointerLife;
using DIVFactor.Spawner;
using UnityEngine;

namespace AetherAlmachina.ActGauge.Pointer
{
    public record PointerSpawnerData(RectTransform Transform, Color Color);

    [RequireComponent(typeof(RectTransform))]
    public abstract class PointerSpawner : MonoBehaviour, ILifetimeSpawner
    {
        RectTransform rect;
        Action<SkillData, PointerSpawnerData> pointerFactory;
        protected abstract Color PointerColor { get; }

        public void SpawnConfigure(SpawnerBuilder builder)
        {
            builder.Register<ActPointerLifetime>(Resources.Load<GameObject>(Path.Combine("Stage", "ActPointer", "Pointer")))
                    .Inject(out pointerFactory);
        }
        void OnEnable()
        {
            rect = GetComponent<RectTransform>();
        }
        public void MakePointer(SkillData skill) => pointerFactory(skill, new(rect, PointerColor));
    }
}