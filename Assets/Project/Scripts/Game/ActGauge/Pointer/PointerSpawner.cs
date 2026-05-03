using System;
using System.IO;
using AetherAlmachina.Skill;
using DConfig.ActPointerLife;
using DIVFactor.Spawner;
using UnityEngine;

namespace AetherAlmachina.ActGauge.Pointer
{
    /// <summary>
    /// 行動ゲージ自体のオブジェクト情報を渡す
    /// </summary>
    /// <param name="Transform">行動ゲージのRect</param>
    /// <param name="Color">ポインターの色</param>
    public record PointerSpawnerData(RectTransform Transform, Color Color);

    /// <summary>
    /// ゲージに攻撃スキルの発動時間を示すポインターをスポーンさせるスポナー
    /// </summary>
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

        /// <summary>
        /// ポインターを生成し、ゲージに乗せる
        /// </summary>
        /// <param name="skill">そのポインターに乗せるスキル</param>
        public void MakePointer(SkillData skill) => pointerFactory(skill, new(rect, PointerColor));
    }
}