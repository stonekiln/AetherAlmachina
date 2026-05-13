using System;
using UnityEngine;
using VContainer;

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