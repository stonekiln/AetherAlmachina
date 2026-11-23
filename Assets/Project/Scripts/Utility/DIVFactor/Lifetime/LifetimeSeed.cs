using System;
using UnityEngine;
using VContainer;
using DIVFactor.Pooling;

namespace DIVFactor.Lifetime
{
    /// <summary>
    /// そのLifetimeObjectの初期化情報を持つ
    /// </summary>
    public class LifetimeSeed : MonoBehaviour
    {
        /// <summary>
        /// 親のLifetimeObject
        /// </summary>
        public LifetimeObject Parent { get; private set; }
        /// <summary>
        /// LifetimeScopeのConfigure内で呼び出されるコールバック
        /// </summary>
        public Action<IContainerBuilder> CallBack { get; set; }
        internal LifetimeArguments Arguments { get; private set; }
        internal LifetimePool Pool { get; set; }
        internal bool DeferStart { get; set; }
        internal Transform SpawnParent { get; set; }
        internal LifetimeSeed SetArguments(LifetimeArguments arguments)
        {
            Arguments = arguments;
            return this;
        }
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
