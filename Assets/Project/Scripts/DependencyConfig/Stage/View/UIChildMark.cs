using DIVFactor.Injectable;
using DIVFactor.Lifecycle;
using DIVFactor.Lifetime;
using UnityEngine;

namespace AetherAlmachina.Stage.UI
{
    public class UIChildMark : MonoBehaviour, IViewUpdater
    {
        PlayerUIRoot destination;
        LifetimeOwnership ownership;

        public void Injection(InjectableResolver resolver)
        {
            resolver.Inject(out destination);
            resolver.Inject(out ownership);
        }

        public void UpdateView(ViewContext context)
        {
            // Transformの親と、表示・破棄を担当するLifetimeの所属は別に保つ。
            ownership.Track(gameObject);
            transform.SetParent(destination.transform, false);
        }
    }
}
