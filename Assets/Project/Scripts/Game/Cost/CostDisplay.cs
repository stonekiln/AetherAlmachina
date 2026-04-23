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
    public class CostDisplay : MonoBehaviour, IInjectable
    {
        TextMeshProUGUI textMeshPro;
        Player owner;

        public void Injection(InjectableResolver resolver)
        {
            resolver.Inject(out owner);

            owner.Status.MPFluctuation.Subscribe(log => UpdateDisplay(owner.Status.magicPoint)).AddTo(this);
        }

        void Awake()
        {
            textMeshPro = gameObject.GetComponent<TextMeshProUGUI>();
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