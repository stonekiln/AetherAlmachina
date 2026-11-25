using System;
using DIVFactor.Lifetime.Root;
using DIVFactor.Pooling;
using UnityEngine;
namespace DIVFactor.Mold
{
    [DefaultExecutionOrder(-10000)]
    [DisallowMultipleComponent]
    public sealed class MoldSceneBootstrap : MonoBehaviour
    {
        [SerializeField, HideInInspector] MoldMetadata metadata;
        [SerializeField, HideInInspector] PrefabCatalog catalog;
        bool attached;
        void Awake()
        {
            if (!metadata || !catalog) throw new InvalidOperationException("DIVFactorのシーン情報をEditorで生成してください。");
            MoldObjects.Attach(catalog);
            attached = true;
            RootLifetimeScope scope = null;
            try
            {
                scope = gameObject.AddComponent<RootLifetimeScope>();
                var arguments = LifetimeArguments.Create();
                scope.Prepare(null, metadata, catalog, arguments, null, null, true);
                scope.BuildPrepared();
                scope.BeginUse(arguments);
            }
            catch
            {
                MoldObjects.Detach(catalog);
                attached = false;
                if (scope) scope.DestroyForOwner();
                throw;
            }
        }
        void OnDestroy() { if (attached) MoldObjects.Detach(catalog); }
#if UNITY_EDITOR
        public void EditorConfigure(MoldMetadata metadata, PrefabCatalog catalog)
        { this.metadata = metadata; this.catalog = catalog; }
#endif
    }
}
