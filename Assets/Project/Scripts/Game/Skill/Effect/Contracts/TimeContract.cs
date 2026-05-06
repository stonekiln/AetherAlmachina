using System;
using AetherAlmachina.Entities;
using R3;

namespace AetherAlmachina.Skill.Effect.Contracts
{
    /// <summary>
    /// 時間(sec)によってModifierが解除される
    /// </summary>
    [Serializable]
    public class TimeContract : EnchantContract
    {
        public override void Sign(IEntityInteraction user, IEntityInteraction target, Action dispel)
        {
            Observable.Timer(TimeSpan.FromSeconds(During)).Subscribe(_ => dispel());
        }
    }
}