using System;
using DConfig.Battle.Event;
using DivFacter.Injectable;
using R3;
using UnityEngine;

/// <summary>
/// カードをInstanceateするためのクラス
/// </summary>
public class CardCreator : MonoBehaviour, IInjectable
{
    CardCreateEventBundle CardCreate;
    Func<SkillData, GameObject> Create;

    public void InjectDependencies(InjectableResolver resolver)
    {
        resolver.Inject(out CardCreate);
        resolver.Inject(out Create);
    }

    void OnEnable()
    {
        CardCreate.Request.Subscribe(request => CardCreate.Response.Publish(new(Create(request.Data)))).AddTo(this);
    }
}