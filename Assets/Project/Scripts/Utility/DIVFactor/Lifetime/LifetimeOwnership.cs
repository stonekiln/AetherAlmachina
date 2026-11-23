using System;
using System.Collections.Generic;
using UnityEngine;

namespace DIVFactor.Lifetime
{
    /// <summary>Transformの親変更後も、生成元が表示と破棄を担当する対象。</summary>
    public sealed class LifetimeOwnership : IDisposable
    {
        readonly List<GameObject> objects = new();
        bool disposed;
        public IReadOnlyList<GameObject> Objects => objects.AsReadOnly();

        public void Track(GameObject ownedObject)
        {
            if (disposed) throw new ObjectDisposedException(nameof(LifetimeOwnership));
            if (!ownedObject) throw new ArgumentNullException(nameof(ownedObject));
            if (objects.Contains(ownedObject)) return;
            if (ownedObject.activeSelf) ownedObject.SetActive(false);
            objects.Add(ownedObject);
        }

        internal void SetVisible(bool visible)
        {
            foreach (GameObject ownedObject in objects)
                if (ownedObject && ownedObject.activeSelf != visible) ownedObject.SetActive(visible);
        }

        public void Dispose()
        {
            if (disposed) return;
            disposed = true;
            foreach (GameObject ownedObject in objects) if (ownedObject) UnityEngine.Object.Destroy(ownedObject);
            objects.Clear();
        }
    }
}
