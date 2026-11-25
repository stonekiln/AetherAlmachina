using System;
using System.Collections.Generic;
using System.Linq;
using DIVFactor.Injectable;

namespace DIVFactor.Lifecycle
{
    /// <summary>分類はEditorで保存し、使用期間を繰り返しても再分類しない。</summary>
    public sealed class LifecycleParticipants
    {
        internal readonly IInjectable[] Injectables;
        internal readonly IViewUpdatable[] Views;
        internal readonly IUsageSubscriber[] Subscribers;
        internal readonly IReleasable[] Releasables;
        public LifecycleParticipants(IInjectable[] injectables,
            IViewUpdatable[] views, IUsageSubscriber[] subscribers, IReleasable[] releasables)
        {
            Injectables = injectables;
            Views = views;
            Subscribers = subscribers;
            Releasables = releasables;
        }
        public static LifecycleParticipants From(IEnumerable<object> participants)
        {
            object[] targets = participants?.ToArray() ?? throw new ArgumentNullException(nameof(participants));
            return new(targets.OfType<IInjectable>().ToArray(),
                targets.OfType<IViewUpdatable>().ToArray(), targets.OfType<IUsageSubscriber>().ToArray(),
                targets.OfType<IReleasable>().ToArray());
        }
    }
}
