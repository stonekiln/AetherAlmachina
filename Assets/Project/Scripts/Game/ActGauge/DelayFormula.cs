using UnityEngine;

namespace AetherAlmachina.ActGauge
{
    /// <summary>
    /// 攻撃スキルをの発動時間を計算するクラス
    /// </summary>
    public class DelayFormula
    {
        /// <summary>
        /// 速度が0の時の発動時間
        /// </summary>
        const float MaxDelayTime = 30f;
        const float Base = 3f;
        const float Damping = 0.01f;

        /// <summary>
        /// 発動時間の計算値を出す
        /// </summary>
        /// <param name="speed">発動時間はエンティティの速度パラメータを参照する</param>
        /// <returns></returns>
        public float GetTime(float speed)
        {
            //速度が100のとき発動時間が10秒になる
            //速度が大きくなるほど発動時間が0に収束する
            return MaxDelayTime / Mathf.Pow(Base, Damping * speed);
        }
    }
}