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

        protected override ModifierData TransformData(EnchantExecutionContext context, ModifierRawData data)
        {
            return new(data,
                preMax =>
                {
                    //FIX:付与された直後からTimeContractのカウントが始まるが
                    //  こちらは付与された直後に毒ダメージが発生しないので10秒間の効果を付与しても9回分のダメージしか発生しない
                    if (preMax == 0)
                    {
                        Observable.Interval(TimeSpan.FromSeconds(1f))
                            .Select(_ => Mathf.FloorToInt(context.Target.Status.GetModifiers<TriggerModifierStock>().Modifiers[data.ModifierType][data.Polarity.GetType()].Max()))
                                .TakeWhile(value => value != 0).Subscribe(value =>
                                    {
                                        context.Target.Interaction.ResourceUpdate.HP.Request.OnNext(new(value));
                                        Debug.Log(context.Target.Name + "が" + Mathf.Abs(value) + "ダメージを受けました。\n残りHP:" + context.Target.Status.Resource.HitPoint);
                                    }).AddTo((Entity)context.Target);
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