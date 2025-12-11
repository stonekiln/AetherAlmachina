using DConfig.Battle.Event;
using DivFacter.Injectable;
using UnityEngine;

/// <summary>
/// それぞれのエンティティのデッキを管理するクラス
/// </summary>
public class DeckManager : MonoBehaviour,IInjectable
{
    DeckEventBundle PlayerDeck;
    DeckEventBundle EnemyDeck;

    public void InjectDependencies(InjectableResolver resolver)
    {
        resolver.Inject(out PlayerDeck);
        resolver.Inject(out EnemyDeck);
    }

    void OnEnable()
    {
        PlayerDeck.Subscribe(this);
        EnemyDeck.Subscribe(this);
    }
}