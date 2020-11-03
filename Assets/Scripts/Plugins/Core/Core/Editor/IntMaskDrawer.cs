using UnityEngine;
using UnityEditor;

namespace Core
{
    [CustomPropertyDrawer(typeof(IntMask))]
    public class IntMaskDrawer : PropertyDrawer
    {
        public readonly string[] displayedOptions = new string[]
        {
            "1", "2", "3", "4", "5", "6", "7", "8", "9", "10",
            "11", "12", "13", "14", "15", "16", "17", "18", "19", "20",
            "21", "22", "23", "24", "25", "26", "27", "28", "29", "30",
            "31", "32"
        };

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            property.intValue = EditorGUI.MaskField(new Rect(position.x, position.y, position.width * 0.5f, EditorGUIUtility.singleLineHeight)
                , property.displayName, property.intValue, displayedOptions);

            property.intValue = EditorGUI.IntField(new Rect(position.x + position.width * 0.5f + EditorGUIUtility.standardVerticalSpacing, position.y, position.width * 0.5f - EditorGUIUtility.standardVerticalSpacing, EditorGUIUtility.singleLineHeight)
                , property.intValue);
        }
    }
}