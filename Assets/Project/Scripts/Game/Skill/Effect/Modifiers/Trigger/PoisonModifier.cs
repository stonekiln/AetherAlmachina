using System;
using AetherAlmachina.Entities;
using R3;
using UnityEngine;

namespace AetherAlmachina.Skill.Effect.Modifiers
{
    /// <summary>
    /// 毒付与のModifierの定義
    /// </summary>
    [Serializable]
    public class PoisonModifier : TriggerModifier
    {
        public override string DisplayUnit => "";

        protected override ModifierData TransformData(IEntityEnchantInteraction user, IEntityEnchantInteraction target, ModifierRawData data)
        {
            return new(data,
                preMax =>
                {
                    Entity entity = (Entity)target;
                    if (preMax == 0)
                    {
                        Observable.Interval(TimeSpan.FromSeconds(1f))
                            .Select(_ => Mathf.FloorToInt(target.Status.GetModifiers<TriggerModifierStock>().Modifiers[data.ModifierType][data.Polarity.GetType()].Max()))
                                .TakeWhile(value => value != 0).Subscribe(value =>
                                    {
                                        target.Process.ResourceUpdate.HP.Request.OnNext(new(value));
                                        Debug.Log(entity.name + "が" + Mathf.Abs(value) + "ダメージを受けました。\n残りHP:" + entity.Status.Resource.HitPoint);
                                    }).AddTo(entity);
                    }

                    Debug.Log(data.TypeData.Name + ":" + data.Value + data.TypeData.DisplayUnit + " の効果が付与された。");
                },
                _ =>
                {
                    Debug.Log(data.TypeData.Name + ":" + data.Value + data.TypeData.DisplayUnit + " の効果が解除された。");
                });
        }
    }
}