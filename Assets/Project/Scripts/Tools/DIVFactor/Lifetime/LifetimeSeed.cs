using System;
using UnityEngine;
using VContainer;

namespace DIVFactor.Lifetime
{
    public class LifetimeSeed : MonoBehaviour
    {
        public LifetimeObject Parent { get; private set; }
        public Action<IContainerBuilder> CallBack { get; set; }
        public void Create(LifetimeObject scope, GameObject prefabObject)
        {
            Parent = scope;
            Instantiate(prefabObject, transform, false).SetActive(false);
        }
        public LifetimeSeed SetCallback(Action<IContainerBuilder> callback)
        {
            CallBack = callback;
            return this;
        }
    }
}