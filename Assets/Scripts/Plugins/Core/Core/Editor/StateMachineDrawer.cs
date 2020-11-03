using UnityEngine;
using UnityEditor;

namespace Core
{
    [CustomPropertyDrawer(typeof(ShowStateAttribute))]
    public class StateMachineDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var stateLable = property.FindPropertyRelative("currentStateLabel");
            if (stateLable != null)
            {
                GUI.enabled = false;
                var rect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
                EditorGUI.PropertyField(rect, stateLable, true);
                GUI.enabled = true;
            }
            /*EditorGUI.Popup(rect, property.displayName, stateLable.intValue, stateLable.enumDisplayNames);*/
        }
	
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            //if (property.isExpanded)
            return property.FindPropertyRelative("currentStateLabel") == null ? 0.0f : EditorGUI.GetPropertyHeight(property);
        }
    }
}