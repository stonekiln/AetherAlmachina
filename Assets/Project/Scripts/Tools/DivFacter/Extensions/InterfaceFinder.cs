using System.Collections.Generic;
using System.Linq;
using DivFacter.Binder;
using DivFacter.Injectable;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DivFacter.Extensions
{
    public static class InterfaceFinder
    {
        /// <summary>
        /// Scene内のオブジェクトからIInjectableを探す
        /// </summary>
        public static IEnumerable<IInjectable> FindInjectable(this Scene scene)
        {
            return scene.GetRootGameObjects().Select(rootGameObject => rootGameObject.GetComponentsInChildren<MonoBehaviour>().OfType<IInjectable>()).SelectMany(monoBehaviour => monoBehaviour);
        }
        /// <summary>
        /// Transform内のオブジェクトからIInjectableを探す
        /// </summary>
        public static IEnumerable<IInjectable> FindInjectable(this Transform transform)
        {
            return transform.GetComponentsInChildren<MonoBehaviour>().OfType<IInjectable>();
        }
        /// <summary>
        /// Scene内のオブジェクトからIInjectableを探す
        /// </summary>
        public static IEnumerable<IObjectBinderBase> FindBinder(this Scene scene)
        {
            return scene.GetRootGameObjects().Select(rootGameObject => rootGameObject.GetComponentsInChildren<MonoBehaviour>().OfType<IObjectBinderBase>()).SelectMany(monoBehaviour => monoBehaviour);
        }
        /// <summary>
        /// Transform内のオブジェクトからIInjectableを探す
        /// </summary>
        public static IEnumerable<IObjectBinderBase> FindBinder(this Transform transform)
        {
            return transform.GetComponentsInChildren<MonoBehaviour>().OfType<IObjectBinderBase>();
        }
    }
}