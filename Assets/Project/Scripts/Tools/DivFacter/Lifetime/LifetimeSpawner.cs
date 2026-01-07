using System;
using DConfig.EnemyLife;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public abstract class LifetimeSpawner : LifetimeScope
{
    GameObject PrefabObject;
    public void RegisterPrefabObject(GameObject gameObject)
    {
        PrefabObject = gameObject;
    }
    protected abstract void ParentConfigure(IContainerBuilder builder);
    protected abstract void ChildConfigure(IContainerBuilder builder);
    protected override void Configure(IContainerBuilder builder)
    {
        ParentConfigure(builder);
        CreateChild(builder => ChildConfigure(builder));
    }
}