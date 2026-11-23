using DIVFactor.Lifecycle;
using AetherAlmachina.Entities.Faction;
using DIVFactor.Injectable;
using R3;
using TMPro;
using UnityEngine;

namespace AetherAlmachina.Cost
{
    /// <summary>
    /// あるエンティティのコストを表示するためのクラス
    /// </summary>
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class CostDisplay : MonoBehaviour, IReactiveView
    {
        TextMeshProUGUI textMeshPro;
        Player owner;

        public void Injection(InjectableResolver resolver)
        {
            resolver.Inject(out owner);

        }

        public void Subscribe(UsageLifetime lifetime)
        {
            owner.Interaction.ResourceUpdate.Cost.Response.Subscribe(log => UpdateDisplay(log.Current)).AddTo(lifetime);
        }

        public void Initialize()
        {
            textMeshPro = GetComponent<TextMeshProUGUI>();
        }

        public void UpdateView() => UpdateDisplay(owner.Status.Resource.Cost);

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