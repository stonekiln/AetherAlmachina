using System;
using DivFacter.Injectable;
using UnityEngine;

namespace DivFacter.Lifetime
{
    public interface ILifetimeSpawner
    {
        public void SpawnConfigure(ObjectBuilder builder);
    }
}