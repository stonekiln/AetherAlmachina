using System.Linq;
using DIVFactor.Mold;
using DIVFactor.Spawner;
using UnityEditor;
using UnityEngine;

namespace AetherAlmachina.Editor.DIVFactor
{
    [CustomEditor(typeof(MoldMetadata))]
    sealed class MoldMetadataInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            MoldMetadata metadata = (MoldMetadata)target;
            EditorGUILayout.LabelField("事前生成", metadata.IsBaked ? "生成済み" : "未生成");
            foreach (MonoBehaviour component in metadata.Components)
                if (component)
                {
                    EditorGUILayout.ObjectField(component.GetType().Name, component, typeof(MonoBehaviour), true);
                    string contracts = string.Join(", ", component.GetType().GetInterfaces()
                        .Where(contract => contract.Namespace != null && contract.Namespace.StartsWith("DIVFactor"))
                        .Select(contract => contract.Name));
                    if (contracts.Length > 0) EditorGUILayout.LabelField("  " + contracts, EditorStyles.wordWrappedLabel);
                }
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Factory → Mold", EditorStyles.boldLabel);
            foreach (FactoryBinding binding in metadata.Bindings)
                EditorGUILayout.LabelField((binding.Owner ? binding.Owner.GetType().Name : "Missing") + "." + binding.FieldName, binding.Path);
        }
    }

    [CustomEditor(typeof(PrefabCatalog))]
    sealed class PrefabCatalogInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            PrefabCatalog catalog = (PrefabCatalog)target;
            EditorGUILayout.LabelField("シーンから到達できるMold", EditorStyles.boldLabel);
            foreach (MoldEntry entry in catalog.Entries)
                EditorGUILayout.ObjectField(entry.Path, entry.Prefab, typeof(MoldMetadata), false);
            EditorGUILayout.HelpBox("DIVFactor/Mold catalogで参照元を確認できます。内容は再生成で更新されます。", MessageType.Info);
        }
    }

}
