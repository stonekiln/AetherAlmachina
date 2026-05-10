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
    public class CostDisplay : MonoBehaviour, IInjectable
    {
        TextMeshProUGUI textMeshPro;
        Player owner;

        public void Injection(InjectableResolver resolver)
        {
            resolver.Inject(out owner);

            owner.Process.CostUpdate.Subscribe(log => UpdateDisplay(owner.Status.Resource.Cost)).AddTo(this);
        }

        void Awake()
        {
            textMeshPro = GetComponent<TextMeshProUGUI>();
        }

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