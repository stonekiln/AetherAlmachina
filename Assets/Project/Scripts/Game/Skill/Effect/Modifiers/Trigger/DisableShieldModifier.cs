using System;
using AetherAlmachina.Entities;
using R3;
using UnityEngine;

namespace AetherAlmachina.Skill.Effect.Modifiers
{
    /// <summary>
    /// 攻撃無効化のModifierの定義
    /// </summary>
    [Serializable]
    public class DisableTimesModifier : TriggerModifier
    {
        public override string DisplayUnit => "(回)";
        protected override ModifierData TransformData(IEntityEnchantInteraction user, IEntityEnchantInteraction target, ModifierRawData data)
        {
            return new(data,
                preMax =>
                {
                    int addDisable = Mathf.FloorToInt(data.ModifyValue);
                    if (target.Status.Resource.Disable < addDisable)
                    {
                        target.Process.ResourceUpdate.Disable.Request.OnNext(new(addDisable - target.Status.Resource.Disable));
                    }

                    Debug.Log(data.TypeData.Name + ":" + data.Value + data.TypeData.DisplayUnit + " の効果が付与された。");
                },
                curMax =>
                {
                    int curDisable = Mathf.FloorToInt(curMax);
                    if (curDisable < target.Status.Resource.Disable)
                    {
                        target.Process.ResourceUpdate.Disable.Request.OnNext(new(curDisable - target.Status.Resource.Disable));
                    }

                    Debug.Log(data.TypeData.Name + ":" + data.Value + data.TypeData.DisplayUnit + " の効果が解除された。");
                })
            {
                ModifierType = typeof(DisableTimesModifier)
            };
        }
        public override Observable<Unit> CreateContract(IEntityEnchantInteraction user, IEntityEnchantInteraction target)
        {
            return target.Process.ResourceUpdate.Disable.Response.Where(log => log.Current <= 0).Take(1).AsUnitObservable();
        }
    }
}