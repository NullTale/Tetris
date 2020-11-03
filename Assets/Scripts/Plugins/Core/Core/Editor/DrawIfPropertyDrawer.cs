using UnityEditor;
using UnityEngine;

namespace Core
{
    [CustomPropertyDrawer(typeof(DrawIfAttribute))]
    public class DrawIfPropertyDrawer : PropertyDrawer
    {
        // Reference to the attribute on the property.
        private DrawIfAttribute drawIf;
 
        // Field that is being compared.
        private SerializedProperty comparedField;

        //////////////////////////////////////////////////////////////////////////
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (ShowMe(property) == false && drawIf.m_DisablingType == DrawIfAttribute.DisablingType.DontDraw)
                return 0f;
   
            // The height of the property should be defaulted to the default height.
            return EditorGUI.GetPropertyHeight(property, label, true);
            //return base.GetPropertyHeight(property, label);
        }
 
        /// <summary>
        /// Errors default to showing the property.
        /// </summary>
        private bool ShowMe(SerializedProperty property)
        {
            drawIf = attribute as DrawIfAttribute;
            // Replace property name to the value from the parameter

            foreach (var copareData in drawIf.m_CompareList)
            {
                string path = property.propertyPath.Contains(".") ? System.IO.Path.ChangeExtension(property.propertyPath, copareData.m_PropertyName) : copareData.m_PropertyName;
	 
                comparedField = property.serializedObject.FindProperty(path);
	 
                if (comparedField == null)
                {
                    Debug.LogError("Cannot find property with name: " + path);
                    return true;
                }

				
                // get the value & compare based on types
                switch (comparedField.propertyType)
                { // Possible extend cases to support your own type
                    case SerializedPropertyType.Boolean:
                        if (comparedField.boolValue.Equals(copareData.m_PropertyValue))
                            return true;
                        break;
                    case  SerializedPropertyType.Enum:
                        if (comparedField.enumValueIndex.Equals((int)copareData.m_PropertyValue))
                            return true;
                        break;
                    case  SerializedPropertyType.ObjectReference:
                        if (comparedField.objectReferenceValue != null)
                            return true;
                        break;
                    default:
                        Debug.LogError("Error: " + comparedField.propertyType + " is not supported of " + path);
                        return true;
                }
            }

            return false;
        }
 
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // If the condition is met, simply draw the field.
            if (ShowMe(property))
            {
                EditorGUI.PropertyField(position, property);
            } //...check if the disabling type is read only. If it is, draw it disabled
            else if (drawIf.m_DisablingType == DrawIfAttribute.DisablingType.ReadOnly)
            {
                GUI.enabled = false;
                EditorGUI.PropertyField(position, property);
                GUI.enabled = true;
            }
        }
 
    }
}