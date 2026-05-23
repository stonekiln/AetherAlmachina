using System;
using AetherAlmachina.Entities;
using UnityEngine;

namespace AetherAlmachina.Skill.Effect.Modifiers
{
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
                        target.Process.ResourceUpdate.Disable.OnNext(new(addDisable - target.Status.Resource.Disable));
                    }

                    Debug.Log(data.TypeData.Name + ":" + data.Value + data.TypeData.DisplayUnit + " の効果が付与された。");
                },
                curMax =>
                {
                    int curDisable = Mathf.FloorToInt(curMax);
                    if (curDisable < target.Status.Resource.Disable)
                    {
                        target.Process.ResourceUpdate.Disable.OnNext(new(curDisable - target.Status.Resource.Disable));
                    }

                    Debug.Log(data.TypeData.Name + ":" + data.Value + data.TypeData.DisplayUnit + " の効果が解除された。");
                })
            {
                ModifierType = typeof(DisableTimesModifier)
            };
        }
    }
}