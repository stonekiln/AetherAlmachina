using System;
using UnityEngine;
using VContainer;

namespace DivFacter.Lifetime
{
    public class LifetimeSeed : MonoBehaviour
    {
        public LifetimeObject Parent { get; private set; }
        public Action<IObjectResolver> CallBack { get; set; }
        public void Create(LifetimeObject scope, GameObject prefabObject, Action<IObjectResolver> callBack)
        {
            Parent = scope;
            CallBack = callBack;
            Instantiate(prefabObject, transform, true);
        }
    }
}