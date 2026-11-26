using AetherAlmachina.Entities.Parameter;
using System;

namespace AetherAlmachina.Skill.Effect.Modifiers
{
    /// <summary>
    /// 定数攻撃力変化のModifierの定義
    /// </summary>
    [Serializable]
    public class AttackFlat : FlatModifier
    {
        public override StatusType StatusTypeKey => StatusType.Attack;
    }
    /// <summary>
    /// 割合攻撃力変化のModifierの定義
    /// </summary>
    [Serializable]
    public class AttackRate : RateModifier
    {
        public override StatusType StatusTypeKey => StatusType.Attack;
    }
}