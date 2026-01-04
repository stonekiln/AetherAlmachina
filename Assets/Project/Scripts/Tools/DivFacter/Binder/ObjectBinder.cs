using System;
using System.Collections.Generic;
using DivFacter.EntryPoint;
using DivFacter.Event;
using DivFacter.Injectable;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace DivFacter.Binder
{
    public abstract class ObjectBinder<T> : MonoBehaviour, IObjectBinder<T> where T : MonoBehaviour
    {
        public Type Key => typeof(T);
        List<Transform> Transforms { get; } = new();

        public void Reserve(InjectableResolver resolver)
        {
            resolver.GetComponent<EventBus<InitializeEvent>>().Subscribe(_ => Bind());
        }
        public void Bind()
        {
            Transforms.ForEach(childTransform => childTransform.SetParent(transform, false));
        }
        public void Register(T element)
        {
            Transforms.Add(element.transform);
        }
    }
}