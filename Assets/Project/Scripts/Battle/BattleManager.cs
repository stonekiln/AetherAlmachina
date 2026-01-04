using System;
using DivFacter.Injectable;
using UnityEngine;

public class BattleManager : MonoBehaviour,IInjectable
{
    [SerializeField] StageSettings stageSettings;

    public void InjectDependencies(InjectableResolver resolver)
    {
        Instantiate(stageSettings.Player,transform);
        foreach(GameObject friendly in stageSettings.Friendly)
        {
            Instantiate(friendly,transform);
        }
        foreach(GameObject hostile in stageSettings.Hostile)
        {
            Instantiate(hostile,transform);
        }
    }
}