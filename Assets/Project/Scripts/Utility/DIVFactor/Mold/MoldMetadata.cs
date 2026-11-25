using System;
using System.Collections.Generic;
using System.Linq;
using DIVFactor.Injectable;
using DIVFactor.Lifecycle;
using DIVFactor.Spawner;
using UnityEngine;

namespace DIVFactor.Mold
{
    [Serializable]
    public sealed class FactoryBinding
    {
        [SerializeField] MonoBehaviour owner;
        [SerializeField] string fieldName;
        [SerializeField] string factoryType;
        [SerializeField] string path;
        public MonoBehaviour Owner => owner;
        public string FieldName => fieldName;
        public string FactoryType => factoryType;
        public string Path => path;
        public FactoryBinding(MonoBehaviour owner, string fieldName, string factoryType, string path)
        { this.owner = owner; this.fieldName = fieldName; this.factoryType = factoryType; this.path = path; }
    }

    [DisallowMultipleComponent]
    public sealed class MoldMetadata : MonoBehaviour
    {
        const int CurrentFormat = 2;
        [SerializeField, HideInInspector] int format;
        [SerializeField, HideInInspector] MonoBehaviour[] components = Array.Empty<MonoBehaviour>();
        [SerializeField, HideInInspector] MonoBehaviour[] spawners = Array.Empty<MonoBehaviour>();
        [SerializeField, HideInInspector] MonoBehaviour[] injectables = Array.Empty<MonoBehaviour>();
        [SerializeField, HideInInspector] MonoBehaviour[] views = Array.Empty<MonoBehaviour>();
        [SerializeField, HideInInspector] MonoBehaviour[] subscribers = Array.Empty<MonoBehaviour>();
        [SerializeField, HideInInspector] MonoBehaviour[] releasables = Array.Empty<MonoBehaviour>();
        [SerializeField, HideInInspector] FactoryBinding[] bindings = Array.Empty<FactoryBinding>();
        LifecycleParticipants participants;
        public IReadOnlyList<MonoBehaviour> Components => components;
        public IReadOnlyList<MonoBehaviour> Spawners => spawners;
        public IReadOnlyList<FactoryBinding> Bindings => bindings;
        public bool IsBaked => format == CurrentFormat;
        internal LifecycleParticipants Participants => participants ??= new(
            injectables.Cast<IInjectable>().ToArray(),
            views.Cast<IViewUpdatable>().ToArray(), subscribers.Cast<IUsageSubscriber>().ToArray(),
            releasables.Cast<IReleasable>().ToArray());
        internal FactoryBinding Select(MonoBehaviour owner, Type factoryType, string fieldName)
        {
            if (!IsBaked) throw new InvalidOperationException("Moldの事前生成情報がありません。Editorで再生成してください。");
            FactoryBinding match = null;
            foreach (FactoryBinding binding in bindings)
                if (binding.Owner == owner && binding.FactoryType == factoryType.AssemblyQualifiedName && (fieldName == null || binding.FieldName == fieldName))
                {
                    if (match != null) throw new InvalidOperationException(owner.GetType().Name + ": 同型Factoryが複数あります。Inject(out factory, nameof(factory))で指定してください。");
                    match = binding;
                }
            return match ?? throw new InvalidOperationException(owner.GetType().Name + ": Factory対応表がありません。Mold属性と再生成を確認してください。");
        }
#if UNITY_EDITOR
        public void EditorBake(MonoBehaviour[] targets, FactoryBinding[] factoryBindings)
        {
            components = targets;
            spawners = targets.Where(c => c is ILifetimeSpawner).ToArray();
            injectables = targets.Where(c => c is IInjectable).ToArray();
            views = targets.Where(c => c is IViewUpdatable).ToArray();
            subscribers = targets.Where(c => c is IUsageSubscriber).ToArray();
            releasables = targets.Where(c => c is IReleasable).ToArray();
            bindings = factoryBindings;
            format = CurrentFormat;
            participants = null;
        }
#endif
    }
}
