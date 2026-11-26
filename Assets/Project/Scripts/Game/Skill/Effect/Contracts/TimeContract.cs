using R3;
using System;

namespace AetherAlmachina.Skill.Effect.Contracts
{
    /// <summary>
    /// 時間(sec)によってModifierが解除される
    /// </summary>
    [Serializable]
    public class TimeContract : EnchantContract
    {
        public override Observable<Unit> Create(EnchantExecutionContext context)
        {
            return Observable.Timer(TimeSpan.FromSeconds(During)).AsUnitObservable();
        }
    }
}