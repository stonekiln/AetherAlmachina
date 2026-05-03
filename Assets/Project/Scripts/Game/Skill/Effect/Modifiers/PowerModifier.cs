using System;
using AetherAlmachina.Entities.Parameter;

namespace AetherAlmachina.Skill.Effect.Modifiers
{
    /// <summary>
    /// 与ダメージ変化のModifierの定義
    /// </summary>
    [Serializable]
    public class PowerRate : RateModifier
    {
        public override StatusType StatusTypeKey => StatusType.Power;
    }
}