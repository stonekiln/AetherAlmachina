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

        public void GaugeOn(SkillData data)
        {
            if (data.User is Player)
            {
                playerPointer.MakePointer(data);
            }
            if (data.User is Enemy)
            {
                enemyPointer.MakePointer(data);
            }
        }
    }
}