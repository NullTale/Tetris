using UnityEditor;
using UnityEngine;

namespace Core
{
    [CustomPropertyDrawer(typeof(AnimatorParameterName))]
    public class AnimatorParameterDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var parameterName = property.FindPropertyRelative("m_Name");
            // shot text field
            var fieldValue = EditorGUI.TextField(position, property.displayName, parameterName.stringValue);
            if (fieldValue != parameterName.stringValue)
            {
                // apply
                parameterName.stringValue = fieldValue;
                property.FindPropertyRelative("m_Hash").intValue = Animator.StringToHash(fieldValue);
                property.serializedObject.ApplyModifiedProperties();
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }
}