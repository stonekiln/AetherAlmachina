using AetherAlmachina.Entities.Faction;
using DIVFactor.Injectable;
using DIVFactor.Lifecycle;
using R3;
using TMPro;
using UnityEngine;

namespace AetherAlmachina.Cost
{
    /// <summary>
    /// あるエンティティのコストを表示するためのクラス
    /// </summary>
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class CostDisplay : MonoBehaviour, ISubscribingViewUpdater
    {
        TextMeshProUGUI textMeshPro;
        Player owner;

        public void Injection(InjectableResolver resolver)
        {
            resolver.Inject(out owner);
            resolver.Injected.SubscribeLifecycle(Initialize, resolver.Fail);
        }

        public void Subscribe(SubscriptionContext context)
        {
            owner.Interaction.ResourceUpdate.Cost.Response.Subscribe(log => UpdateDisplay(log.Current)).AddTo(context.Lifetime);
        }

        public void Initialize()
        {
            textMeshPro = GetComponent<TextMeshProUGUI>();
        }

        public void UpdateView(ViewContext context) => UpdateDisplay(owner.Status.Resource.Cost);

        /// <summary>
        /// 画面に現在のMPを表示する
        /// </summary>
        /// <param name="mp">表示する値</param>
        void UpdateDisplay(int mp)
        {
            textMeshPro.text = mp.ToString();
        }
    }
}