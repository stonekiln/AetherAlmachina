using AetherAlmachina.Skill;
using AetherAlmachina.Skill.Effect;
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
            //全フィールドを取得する
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
                    //新しく追加されたエフェクトは初期値を全てnullにした状態で追加する
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

            //説明文は収納可能にする
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

            string QueueLabel = "Effect Queue";
            if (initialLockOnProp.boxedValue is LockOnParameter parameter && parameter.Selector != null && parameter.Selector.IsDeferrable)
            {
                QueueLabel += "(Delay)";
            }
            isQueuePulled = EditorGUILayout.Foldout(isQueuePulled, QueueLabel, true);
            if (isQueuePulled)
            {
                EditorGUI.indentLevel++;
                //SkillEffectのプロパティ表示位置の調整
                Rect rect = EditorGUILayout.GetControlRect();
                rect = new(rect.x + ListMargin, rect.y, rect.width - ListMargin * 2, rect.height);
                EditorGUIUtility.labelWidth += ListMargin * 2;
                //0番目はInitialLockOnとして変更不可に見せる
                using (new EditorGUI.DisabledScope(true))
                {
                    EditorGUI.Popup(rect, "Effect 0", 0, new string[] { "LockOn" });
                }

                rect = EditorGUILayout.GetControlRect(true, EditorGUI.GetPropertyHeight(initialLockOnProp, true));
                rect = new(rect.x + ListMargin, rect.y, rect.width - ListMargin * 2, rect.height);
                //InitialLockOnのパラメータを表示する
                EditorGUI.PropertyField(rect, initialLockOnProp, new("Parameter"), true);

                EditorGUIUtility.labelWidth -= ListMargin * 2;
                effectList.DoLayoutList();

                EditorGUI.indentLevel--;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}