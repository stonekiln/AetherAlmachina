using System;
using AetherAlmachina.Entities;
using R3;
using UnityEngine;

namespace AetherAlmachina.Skill.Effect.Modifiers
{
    [Serializable]
    public class PoisonModifier : TriggerModifier
    {
        public override string DisplayUnit => "";

        protected override TriggerModifierData MakeTriggerData(IEntityInteraction user, IEntityInteraction target, ModifierRawData data)
        {
            return new(data)
            {
                Value = data.Value,
                CallBack = (func) => Observable.Interval(TimeSpan.FromSeconds(1f)).Subscribe(_ =>
                {
                    int damage = func();
                    target.Process.ResourceUpdate.HP.OnNext(new(damage));
                    Entity entity = (Entity)target;
                    Debug.Log(entity.name + "が" + -damage + "ダメージを受けました。\n残りHP:" + entity.Status.Resource.HitPoint);
                })
            };
        }
    }
}