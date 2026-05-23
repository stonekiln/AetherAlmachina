using System.Collections.Generic;
using AetherAlmachina.Entities.Parameter;

namespace AetherAlmachina.Skill.Effect.Modifiers
{
    /// <summary>
    /// Modifierによるパラメータの補正値を管理するクラス
    /// </summary>
    public abstract class ModifierParameter : ModifierStock
    {
        //TODO:辞書型を直接参照する形にしないこと
        /// <summary>
        /// 付与されているModifierの種類
        /// </summary>
        public Dictionary<StatusType, float> ModifierSum { get; init; }

        public ModifierParameter(Dictionary<StatusType, float> status)
        {
            Modifiers = new();
            ModifierSum = new();
            foreach (StatusType key in status.Keys)
            {
                ModifierSum[key] = 0f;
            }
        }

        /// <summary>
        /// 補正値の計算方法
        /// </summary>
        /// <param name="data"></param>
        /// <param name="sumValue"></param>
        /// <returns></returns>
        protected abstract float CalcValue(float value);
        public float GetValue(StatusType statusTypeKey)
        {
            return CalcValue(ModifierSum[statusTypeKey]);
        }
    }
    /// <summary>
    /// 定数変化のModifierによるパラメータの補正値を管理するクラス
    /// </summary>
    public class FlatModifierParameter : ModifierParameter
    {
        public FlatModifierParameter(Dictionary<StatusType, float> status) : base(status) { }

        protected override float CalcValue(float value)
        {
            return value;
        }
    }
    /// <summary>
    /// 割合変化のModifierによるパラメータの補正値を管理するクラス
    /// </summary>
    public class RateModifierParameter : ModifierParameter
    {
        public RateModifierParameter(Dictionary<StatusType, float> status) : base(status) { }

        protected override float CalcValue(float value)
        {
            return 1f + (value / 100f);
        }
    }
}