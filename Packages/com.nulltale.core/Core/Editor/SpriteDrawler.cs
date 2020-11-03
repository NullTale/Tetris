using UnityEditor;
using UnityEngine;

namespace Core
{
    [CustomPropertyDrawer(typeof(SpriteDrawlerAttribute))]
    public class SpriteDrawer : PropertyDrawer
    {
        private static GUIStyle s_TempStyle = new GUIStyle();

        //////////////////////////////////////////////////////////////////////////
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //create object field for the sprite
            var spriteRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            property.objectReferenceValue = EditorGUI.ObjectField(spriteRect, property.displayName, property.objectReferenceValue, typeof(Sprite), false);
 
            // if this is not a repaint or the property is null exit now
            if (Event.current.type != EventType.Repaint || property.objectReferenceValue == null)
                return;
 
            // draw a sprite
            var sp = property.objectReferenceValue as Sprite;
 
            var spriteDrawlerAttribute = attribute as SpriteDrawlerAttribute;

            var width = Mathf.Clamp(Mathf.CeilToInt(spriteDrawlerAttribute.Height * (sp.texture.width / (float)sp.texture.height)), 1, position.width);
            var height = spriteDrawlerAttribute.Height;

            spriteRect.y += EditorGUIUtility.singleLineHeight + 4;
            spriteRect.x = position.x + position.width - width;
            spriteRect.width = width;
            spriteRect.height = height;
		
            s_TempStyle.normal.background = sp.texture;
            s_TempStyle.Draw(spriteRect, GUIContent.none, false, false, false, false);
        }
 
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var spriteDrawlerAttribute = attribute as SpriteDrawlerAttribute;
            return base.GetPropertyHeight(property, label) + EditorGUIUtility.singleLineHeight + 4 + spriteDrawlerAttribute.Height;
        }
    }
}