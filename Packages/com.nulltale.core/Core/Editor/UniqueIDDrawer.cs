using System;
using UnityEditor;
using UnityEngine;

namespace Core
{
    [CustomPropertyDrawer(typeof(UniqueID), true)]
    public class UniqueIDDrawer : PropertyDrawer 
    {
        private static string c_GenerateGuidEvent = "GenerateGuid";
        private static void implGenerateGuid()
        {
            var generateGuidEvet = EditorGUIUtility.CommandEvent("GenerateGuid");
            EditorWindow.focusedWindow.SendEvent(generateGuidEvet);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var uid = property.FindPropertyRelative("m_GUID");
		
		
            // assign new guid
            if (string.IsNullOrEmpty(uid.stringValue) ||				// reset if null
                ((Event.current.type == EventType.MouseDown)			// or double click)
                 && (Event.current.button == 0)
                 && (Event.current.clickCount == 2))
                && new Rect(position.x + EditorGUIUtility.labelWidth, position.y, position.width - EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight).Contains(Event.current.mousePosition))
            {
                uid.stringValue = Guid.NewGuid().ToString();
                uid.serializedObject.ApplyModifiedProperties();
            }

            // repaint
            if (Event.current.type == EventType.Repaint)
            {
                EditorGUI.LabelField(position, new GUIContent("GUID"), new GUIContent(uid.stringValue, "Double click to generate GUID"));
            }

            if (Event.current.type == EventType.ExecuteCommand)
            {
                if (Event.current.commandName == c_GenerateGuidEvent)
                {
                    uid.stringValue = Guid.NewGuid().ToString();
                    uid.serializedObject.ApplyModifiedProperties();
                }
            }

            if (Event.current.type == EventType.ContextClick && position.Contains(Event.current.mousePosition))
            {
                var context = new GenericMenu();
		 
                context.AddItem(new GUIContent ("Generate"), false, () => 
                {
                    uid.stringValue = Guid.NewGuid().ToString();
                    uid.serializedObject.ApplyModifiedProperties();
                });
                context.AddItem(new GUIContent ("Copy"), false, () => EditorGUIUtility.systemCopyBuffer = uid.stringValue);
                context.AddItem(new GUIContent ("Paste"), false, () =>
                {
                    uid.stringValue = EditorGUIUtility.systemCopyBuffer;
                    uid.serializedObject.ApplyModifiedProperties();
                });
		 
                context.ShowAsContext();
            }
		
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        /*private void OnEnable()
	{
		var uid = serializedObject.FindProperty("m_UniqueID");
		// Generate a unique ID, defaults to an empty string if nothing has been serialized yet
		if (string.IsNullOrEmpty(uid.stringValue))
		{
			var guid = Guid.NewGuid();
			uid.stringValue = guid.ToString();
			serializedObject.ApplyModifiedPropertiesWithoutUndo();
		}
	}*/
    }
}