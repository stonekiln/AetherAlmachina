using AetherAlmachina.Skill;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Utility;

namespace EditorExtends
{
    [CustomEditor(typeof(SkillAsset))]
    public class SkillAssetEditor : Editor
    {
        const float ListMargin = 7f;
        SerializedProperty scriptProp;
        SerializedProperty nameProp;
        SerializedProperty descriptionProp;
        SerializedProperty costProp;
        SerializedProperty iconProp;
        SerializedProperty initialLockOnProp;
        SerializedProperty effectQueueProp;
        ReorderableList effectList;
        bool isDescriptionPulled = true;
        bool isQueuePulled = true;

        void OnEnable()
        {
            scriptProp = serializedObject.FindProperty("m_Script");
            nameProp = serializedObject.FindProperty(BackingField.Get("SkillName"));
            descriptionProp = serializedObject.FindProperty(BackingField.Get("Description"));
            costProp = serializedObject.FindProperty(BackingField.Get("Cost"));
            iconProp = serializedObject.FindProperty(BackingField.Get("Icon"));
            initialLockOnProp = serializedObject.FindProperty(BackingField.Get("InitialLockOn"));
            effectQueueProp = serializedObject.FindProperty(BackingField.Get("EffectQueue"));

            effectList = new(serializedObject, effectQueueProp, true, false, true, true)
            {
                elementHeightCallback = index =>
                {
                    SerializedProperty element = effectQueueProp.GetArrayElementAtIndex(index);
                    return EditorGUI.GetPropertyHeight(element, true);
                },
                drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    SerializedProperty element = effectQueueProp.GetArrayElementAtIndex(index);
                    rect.height = EditorGUI.GetPropertyHeight(element, true);

                    EditorGUI.PropertyField(rect, element, new($"Effect {index + 1}"), true);
                },
                onAddCallback = list =>
                {
                    list.serializedProperty.InsertArrayElementAtIndex(list.serializedProperty.arraySize);
                    SerializedProperty newEffect = list.serializedProperty.GetArrayElementAtIndex(list.serializedProperty.arraySize - 1);

                    newEffect.FindPropertyRelative(BackingField.Get("Effect")).managedReferenceValue = null;
                    newEffect.FindPropertyRelative(BackingField.Get("Parameter")).managedReferenceValue = null;

                    list.index = list.serializedProperty.arraySize - 1;
                    list.serializedProperty.serializedObject.ApplyModifiedProperties();
                }
            };
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.PropertyField(scriptProp);
            }

            EditorGUILayout.PropertyField(nameProp);

            if (isDescriptionPulled)
            {
                Rect rect = EditorGUILayout.GetControlRect(true, EditorGUI.GetPropertyHeight(descriptionProp));
                rect.height = EditorGUIUtility.singleLineHeight;
                isDescriptionPulled = EditorGUI.Foldout(rect, isDescriptionPulled, "Description", true);

                rect.height = EditorGUI.GetPropertyHeight(descriptionProp);
                EditorGUI.PropertyField(rect, descriptionProp, GUIContent.none);
            }
            else
            {
                Rect rect = EditorGUILayout.GetControlRect();
                isDescriptionPulled = EditorGUI.Foldout(rect, isDescriptionPulled, "Description", true);
            }

            EditorGUILayout.PropertyField(costProp);
            EditorGUILayout.PropertyField(iconProp);

            isQueuePulled = EditorGUILayout.Foldout(isQueuePulled, "Effect Queue", true);
            if (isQueuePulled)
            {
                EditorGUI.indentLevel++;

                Rect rect = EditorGUILayout.GetControlRect();
                rect = new(rect.x + ListMargin, rect.y, rect.width - ListMargin * 2, rect.height);
                EditorGUIUtility.labelWidth += ListMargin * 2;

                using (new EditorGUI.DisabledScope(true))
                {
                    EditorGUI.Popup(rect, "Effect 0", 0, new string[] { "LockOn" });
                }

                rect = EditorGUILayout.GetControlRect(true, EditorGUI.GetPropertyHeight(initialLockOnProp, true));
                rect = new(rect.x + ListMargin, rect.y, rect.width - ListMargin * 2, rect.height);

                EditorGUI.PropertyField(rect, initialLockOnProp, new("Parameter"), true);

                EditorGUIUtility.labelWidth -= ListMargin * 2;
                effectList.DoLayoutList();

                EditorGUI.indentLevel--;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}