using AetherAlmachina.ActGauge.Pointer;
using AetherAlmachina.Entities.Faction;
using AetherAlmachina.Skill;
using DConfig.StageLife.Event;
using DIVFactor.Event;
using DIVFactor.Injectable;
using R3;
using UnityEngine;

namespace AetherAlmachina.Stage
{
    /// <summary>
    /// スキルを発動させるための処理を行うためのクラス
    /// </summary>
    public class SkillTriggerManager : MonoBehaviour, IInjectable
    {
        EventBus<SkillActivateEvent> skillActivate;
        PointerSpawner playerPointer;
        PointerSpawner enemyPointer;

        public void Injection(InjectableResolver resolver)
        {
            resolver.Inject(out skillActivate);
            playerPointer = resolver.GetComponent<FriendlyPointer>();
            enemyPointer = resolver.GetComponent<HostilePointer>();

            skillActivate.Subscribe(log => GaugeOn(log.Data)).AddTo(this);
        }

        /// <summary>
        /// 発動に時間がかかるスキルは、このメソッドによって行動ゲージに乗せる
        /// </summary>
        /// <param name="data">ゲージに乗せるスキル</param>
        public void GaugeOn(SkillData data)
        {
            if (data.Owner is Player)
            {
                playerPointer.MakePointer(data);
            }
            if (data.Owner is Enemy)
            {
                enemyPointer.MakePointer(data);
            }
        }
    }
}