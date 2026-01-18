using DivFacter.Injectable;
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
    }

    void Awake()
    {
        textMeshPro = gameObject.GetComponent<TextMeshProUGUI>();
    }
    
    void OnEnable()
    {
        owner.Status.MPfluctuation.Subscribe(log => SetDisplay(owner.Status.magicPoint)).AddTo(this);
    }

    void SetDisplay(int mp)
    {
        textMeshPro.text = mp.ToString();
    }
}