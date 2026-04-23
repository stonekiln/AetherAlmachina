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
        /// <summary>
        /// 設定できる数値の最大値
        /// </summary>
        public abstract float ParameterMax { get; }
        /// <summary>
        /// 設定できる数値の最小値
        /// </summary>
        public abstract float ParameterMin { get; }
    }
    /// <summary>
    /// バフ効果の定義
    /// </summary>
    [Serializable]
    public class PositiveModifier : ModifierPolarity
    {
        public override string DisplaySign => "+";
        public override float ParameterMax => float.PositiveInfinity;
        public override float ParameterMin => 0;
    }
    /// <summary>
    /// デバフ効果の定義
    /// </summary>
    [Serializable]
    public class NegativeModifier : ModifierPolarity
    {
        public override string DisplaySign => "-";
        public override float ParameterMax => 0;
        public override float ParameterMin => float.NegativeInfinity;
    }
}