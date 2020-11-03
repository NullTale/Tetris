using UnityEngine;
using UnityEditor;

namespace Core
{
    [CustomPropertyDrawer(typeof(NormalizedAttribute))]
    public class VectorNormalDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.Vector2:
                {
                    if (property.vector2Value == default)
                        property.vector2Value = (attribute as NormalizedAttribute).m_DefaultValue;

                    var result = EditorGUI.Vector2Field(position, property.displayName, property.vector2Value);
                    property.vector2Value = result.normalized;
                } break;
                case SerializedPropertyType.Vector3:
                {
                    if (property.vector3Value == default)
                        property.vector3Value = (attribute as NormalizedAttribute).m_DefaultValue;

                    var result = EditorGUI.Vector3Field(position, property.displayName, property.vector3Value);
                    property.vector3Value = result.normalized;
                }
                    break;
                case SerializedPropertyType.Vector4:
                {
                    if (property.vector4Value == default)
                        property.vector4Value = (attribute as NormalizedAttribute).m_DefaultValue;

                    var result = EditorGUI.Vector4Field(position, property.displayName, property.vector4Value);
                    property.vector4Value = result.normalized;
                }
                    break;
                default:
                    EditorGUI.PropertyField(position, property);
                    break;
            }
        }
    }
}