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
        protected override Observable<Unit> Create(IEntityInteraction user, IEntityInteraction target)
        {
            return Observable.Timer(TimeSpan.FromSeconds(During)).AsUnitObservable();
        }
    }
}