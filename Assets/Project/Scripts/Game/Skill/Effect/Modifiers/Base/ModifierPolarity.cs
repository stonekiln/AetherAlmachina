using System;

namespace AetherAlmachina.Skill.Effect.Modifiers
{
    /// <summary>
    /// Modifierがバフかデバフか定義する
    /// </summary>
    public abstract class ModifierPolarity
    {
        /// <summary>
        /// 変化量の正負
        /// </summary>
        public abstract string DisplaySign { get; }
        public abstract float Get(float value);
    }
    /// <summary>
    /// バフ効果の定義
    /// </summary>
    [Serializable]
    public class PositiveModifier : ModifierPolarity
    {
        public override string DisplaySign => "+";
        public override float Get(float value) => value;
    }
    /// <summary>
    /// デバフ効果の定義
    /// </summary>
    [Serializable]
    public class NegativeModifier : ModifierPolarity
    {
        public override string DisplaySign => "-";
        public override float Get(float value) => -value;
    }
}