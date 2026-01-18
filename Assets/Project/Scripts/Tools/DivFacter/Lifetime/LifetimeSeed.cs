using System;
using UnityEngine;
using VContainer;

namespace DivFacter.Lifetime
{
    public class LifetimeSeed : MonoBehaviour
    {
        public LifetimeObject Parent { get; private set; }
        public Action<AssetRegister> CallBack { get; set; }
        public void Create(LifetimeObject scope, GameObject prefabObject)
        {
            Parent = scope;
            Instantiate(prefabObject, transform, true);
        }
        public LifetimeSeed SetCallback(Action<AssetRegister> callback)
        {
            CallBack = callback;
            return this;
        }
    }
}