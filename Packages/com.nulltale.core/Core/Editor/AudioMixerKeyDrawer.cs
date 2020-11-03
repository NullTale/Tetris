using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;
using Object = UnityEngine.Object;

namespace Core
{
    [CustomPropertyDrawer(typeof(AudioMixerKeyAttribute))]
    public class AudioMixerKeyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //if (property.propertyType != SerializedPropertyType.String)

            var soundManager = Object.FindObjectOfType<SoundManager>();
            var mixer = soundManager.GetMixer();

            
            if (soundManager != null)
            {
                var keyList = _GetExposedParameters(mixer).ToList();
                var currentKeyIndex = keyList.IndexOf(property.stringValue);
                var presented = currentKeyIndex != -1;
                var controlID = GUIUtility.GetControlID(FocusType.Passive);

                // label
                EditorGUI.LabelField(position, property.displayName);

                // apply selected
                if (ObjectPickerWindow.TryGetPickedObject(controlID, out string picked))
                    property.stringValue = picked;
                
                if (presented)
                {
                    // selection button
                    if (GUI.Button(new Rect(position.x + EditorGUIUtility.labelWidth, position.y, position.width - EditorGUIUtility.labelWidth, position.height), property.stringValue))
                        ObjectPickerWindow.Show(controlID, property.stringValue, keyList, 0, s => new GUIContent(s), "Select an exposed parameter");
                }
                else
                {
                    // change & save color
                    var tmpColor = GUI.backgroundColor;
                    GUI.backgroundColor = Color.red;

                    // selection button
                    if (GUI.Button(new Rect(position.x + EditorGUIUtility.labelWidth, position.y, position.width - EditorGUIUtility.labelWidth, position.height), property.stringValue))
                        ObjectPickerWindow.Show(controlID, property.stringValue, 
                            keyList, 0, s => new GUIContent(s), "Select an exposed parameter");

                    // restore bg color
                    GUI.backgroundColor = tmpColor;
                }
                
                // label click
                if (Event.current.type == EventType.ContextClick 
                    && new Rect(position.x, position.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight)
                        .Contains(Event.current.mousePosition))
                {
                    // create context menu
                    var context = new GenericMenu();
                    
                    // fill the menu, list could be large but for small projects, this is more convenient
                    foreach (var key in keyList)
                    {
                        context.AddItem(new GUIContent(key), key == property.stringValue, setKey, key);

                        // do not create lambda for all items
                        void setKey(object userData)
                        {
                            property.stringValue = (string)userData;
                            property.serializedObject.ApplyModifiedProperties();
                        }
                    }

                    // show menu
                    context.ShowAsContext();
                }
            }
            else
            {
                // show default string
                EditorGUI.PropertyField(position, property, true);
            }
        }

        private IEnumerable<string> _GetExposedParameters(AudioMixer mixer)
        {
            var parameters = (Array)mixer.GetType().GetProperty("exposedParameters")?.GetValue(mixer, null);
            if (parameters == null)
                yield break;
   
            for (var i = 0; i < parameters.Length; i++)
            {
                var o = parameters.GetValue(i);
                var param = (string)o.GetType().GetField("name").GetValue(o);

                yield return param;
            }
        }
    }
}