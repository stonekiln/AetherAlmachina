using DIVFactor.Injectable;
using R3;
using TMPro;
using UnityEngine;

/// <summary>
/// あるエンティティのコストを表示するためのクラス
/// </summary>
public class CostDisplay : MonoBehaviour,IInjectable
{
    TextMeshProUGUI textMeshPro;
    Player owner;

    public void InjectDependencies(InjectableResolver resolver)
    {
        resolver.Inject(out owner);
        
        owner.Status.MPfluctuation.Subscribe(log => SetDisplay(owner.Status.magicPoint)).AddTo(this);
    }

    void Awake()
    {
        textMeshPro = gameObject.GetComponent<TextMeshProUGUI>();
    }

    void SetDisplay(int mp)
    {
        textMeshPro.text = mp.ToString();
    }
}