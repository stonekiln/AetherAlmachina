using DIVFactor.Lifecycle;
using System;
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
    public abstract class PointerSpawner : MonoBehaviour, ILifetimeSpawner, IInitializable
    {
        RectTransform rect;
        [Mold("Stage/ActPointer/Pointer")]
        ActionFactory<ActivatedSkillData, PointerSpawnerData> pointerFactory;
        protected abstract Color PointerColor { get; }

        public void SpawnConfigure(SpawnerBuilder builder)
        {
            builder.RegisterPooled<ActPointerLifetime>()
                    .Inject(out pointerFactory);
        }
        public void Initialize()
        {
            rect = GetComponent<RectTransform>();
        }

        /// <summary>
        /// ポインターを生成し、ゲージに乗せる
        /// </summary>
        /// <param name="skill">そのポインターに乗せるスキル</param>
        public void MakePointer(ActivatedSkillData skill) => pointerFactory.Spawn(skill, new(rect, PointerColor));
    }
}
