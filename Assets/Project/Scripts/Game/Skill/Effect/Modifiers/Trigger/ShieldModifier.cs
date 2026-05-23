using System;
using AetherAlmachina.Entities;
using AetherAlmachina.Entities.Parameter;
using UnityEngine;

namespace AetherAlmachina.Skill.Effect.Modifiers
{
    public abstract class ShieldModifier : TriggerModifier
    {
        protected override ModifierData TransformData(IEntityEnchantInteraction user, IEntityEnchantInteraction target, ModifierRawData data)
        {
            return new(data,
                preMax =>
                {
                    int addShield = Mathf.FloorToInt(data.ModifyValue);
                    if (target.Status.Resource.Shield < addShield)
                    {
                        target.Process.ResourceUpdate.Shield.OnNext(new(addShield - target.Status.Resource.Shield));
                    }

                    Debug.Log(data.TypeData.Name + ":" + data.Value + data.TypeData.DisplayUnit + " の効果が付与された。");
                },
                curMax =>
                {
                    int curShield = Mathf.FloorToInt(curMax);
                    if (curShield < target.Status.Resource.Shield)
                    {
                        target.Process.ResourceUpdate.Shield.OnNext(new(curShield - target.Status.Resource.Shield));
                    }

                    Debug.Log(data.TypeData.Name + ":" + data.Value + data.TypeData.DisplayUnit + " の効果が解除された。");
                })
            {
                ModifierType = typeof(ShieldModifier)
            };
        }
    }
    /// <summary>
    /// 定数攻撃力変化のModifierの定義
    /// </summary>
    [Serializable]
    public class ShieldFlat : ShieldModifier
    {
        public override string DisplayUnit => "";
    }
    /// <summary>
    /// 割合攻撃力変化のModifierの定義
    /// </summary>
    [Serializable]
    public class ShieldHPRate : ShieldModifier
    {
        public override string DisplayUnit => "%";
        protected override ModifierData TransformData(IEntityEnchantInteraction user, IEntityEnchantInteraction target, ModifierRawData data)
        {
            ModifierRawData modifiedValueData = new(data)
            {
                ModifyValue = user.Status.Get(StatusType.MaxHitPoint) * data.ModifyValue / 100f
            };
            return base.TransformData(user, target, modifiedValueData);
        }
    }
}