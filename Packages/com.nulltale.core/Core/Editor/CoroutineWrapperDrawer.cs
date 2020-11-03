using System.Collections;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Core
{
    [CustomPropertyDrawer(typeof(CoroutineWrapper))]
    public class CoroutineWrapperDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            property.isExpanded = EditorGUI.Foldout(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight),
                property.isExpanded, new GUIContent(property.displayName), true);

            if (property.isExpanded)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    // object
                    var ownerField = property.FindPropertyRelative("m_Owner");
                    EditorGUI.ObjectField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight),
                        ownerField);
                
                    // function
                    if (ownerField.objectReferenceValue != null)
                    {
                        var functionNameField = property.FindPropertyRelative("m_EnumeratorFunctionName");
                        // get only (IEnumerator ..(Void)) functions
                        var methods = ownerField.objectReferenceValue
                            .GetType()
                            .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                            .Where(n => n.ReturnType == typeof(IEnumerator))
                            .ToList();

                        // prepare data for popup
                        var methodInfo = methods.FirstOrDefault((n) => n.Name == functionNameField.stringValue);
                        var index = methodInfo == null ? 0 : methods.IndexOf(methodInfo) + 1;
                        var options = methods.Select(n => n.Name).Prepend("Null").ToArray();

                        // finally show popup
                        var selected = EditorGUI.Popup(
                            new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 2.0f, position.width,
                                EditorGUIUtility.singleLineHeight),
                            "Function", index, options);

                        // update string value
                        if (selected != index)
                        {
                            if (selected == 0)
                                functionNameField.stringValue = "";
                            else
                                functionNameField.stringValue = options[selected];

                            functionNameField.serializedObject.ApplyModifiedProperties();
                        }
                    }
                }
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.isExpanded == false)
                return EditorGUIUtility.singleLineHeight;

            // do not show method string if owner not set
            return EditorGUIUtility.singleLineHeight * (property.FindPropertyRelative("m_Owner").objectReferenceValue == null ? 2.0f : 3.0f);
        }
    }
}