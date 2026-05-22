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

        protected override TriggerModifierData MakeModifierData(IEntityInteraction user, IEntityInteraction target, ModifierData data)
        {
            return new(data)
            {
                Value = data.Value,
                ApplyCallBack = (value) => Observable.Interval(TimeSpan.FromSeconds(1f)).Subscribe(_ =>
                {
                    target.Process.ResourceUpdate.HP.OnNext(new(value));
                    Entity entity = (Entity)target;
                    Debug.Log(entity.name + "が" + -value + "ダメージを受けました。\n残りHP:" + entity.Status.Resource.HitPoint);
                }),
                DispelCallBack = ()
            };
        }
    }
}