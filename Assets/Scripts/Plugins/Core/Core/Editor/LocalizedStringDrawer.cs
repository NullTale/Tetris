using System;
using System.Linq;
using Core.Module;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Core
{
    [CustomPropertyDrawer(typeof(LocalizedString))]
    public class LocalizedStringDrawer : PropertyDrawer
    {
        private const int	c_LanguagePopupWidth = 60;
        private const int	c_SelectButtonWidth = 22;

        //////////////////////////////////////////////////////////////////////////
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            switch (Event.current.type)
            {
                case EventType.MouseMove:
                case EventType.Layout:
                case EventType.DragUpdated:
                case EventType.DragPerform:
                case EventType.DragExited:
                case EventType.Ignore:
                case EventType.Used:
                //case EventType.ValidateCommand:
                //case EventType.ExecuteCommand:
                    return;

                default:
                    break;
            }

            // try get localization manager
            var core = Object.FindObjectOfType<Core>();
            if (core == null)
            {
                EditorGUI.LabelField(position, "Core not presented on scene");
                return;
            }

            var localizationManager = core.GetModule<Localization>()?.GetEditorLocalizationManager();
            if (localizationManager == null)
            {
                EditorGUI.LabelField(position, "Core doesn't include active serialization module");
                return;
            }
            
            var key = property.FindPropertyRelative(LocalizedString.c_KeyProperty);
            var hasKey = localizationManager.ContainsKey(key.stringValue);
            var controlID = GUIUtility.GetControlID(FocusType.Passive);
            
            // select existing key from menu
            ObjectPickerWindow.TryGetPickedObject(controlID, out string picked);
            if (picked != null)
            {
                key.stringValue = picked;
                property.serializedObject.ApplyModifiedProperties();
            }

            // in header rect
            if (Event.current.type == EventType.ContextClick 
            && new Rect(position.x, position.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight)
                .Contains(Event.current.mousePosition))
            {
                // create context menu
                var context = new GenericMenu();
                
                // select existing key
                context.AddItem(new GUIContent ("Select"), false, () =>
                {
                    // ReSharper disable once ConvertClosureToMethodGroup
                    selectKey();
                });

                // if presented in dictionary key
                if (hasKey)
                {
                    // rename key
                    context.AddItem(new GUIContent("Rename"), false, () =>
                    {
                        TextEditWindow.Show(controlID, key.stringValue, s =>
                        {
                            if (string.IsNullOrWhiteSpace(s))
                                return;

                            if (s == key.stringValue)
                                return;

                            // rename key if the name is not taken
                            if (localizationManager.ChangeKey(key.stringValue, s, LocalizationManager.ChangeLocalizationKeyMode.Block))
                            {
                                key.stringValue = s;
                                property.serializedObject.ApplyModifiedProperties();

                                // reload
                                reloadDictionary();
                            }
                        });
                    });
                    // delete key
                    context.AddItem(new GUIContent("Delete"), false, () =>
                    {
                        if (string.IsNullOrWhiteSpace(key.stringValue))
                            return;

                        // remove key, save changes
                        localizationManager.RemoveKey(key.stringValue);
                        key.stringValue = string.Empty;
                        property.serializedObject.ApplyModifiedProperties();

                        // reload
                        reloadDictionary();
                    });
                }
                // refresh database
                context.AddItem(new GUIContent("Refresh"), false, () =>
                {
                    // ReSharper disable once ConvertClosureToMethodGroup
                    reloadDictionary();
                });

                // show context window
                context.ShowAsContext();
            }
            
            // foldout check
            property.isExpanded = EditorGUI.Foldout(new Rect(position.x, position.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight),
                property.isExpanded, property.displayName, true);

            if (property.isExpanded)
            {
                // key edit, update text
                var keyValue = EditorGUI.TextField(
                    new Rect(position.x + EditorGUIUtility.labelWidth, position.y, position.width - EditorGUIUtility.labelWidth - c_LanguagePopupWidth - c_SelectButtonWidth, EditorGUIUtility.singleLineHeight),
                    "", key.stringValue);
                    
                // is key changed
                if (keyValue != key.stringValue)
                {
                    // rename key
                    if (localizationManager.ChangeKey(key.stringValue, keyValue,
                        LocalizationManager.ChangeLocalizationKeyMode.None))
                    {
                        // save changes
                        key.stringValue = keyValue;
                        key.serializedObject.ApplyModifiedProperties();
                    }
                }

                // select key button
                if (GUI.Button(new Rect(position.x + position.width - c_LanguagePopupWidth - c_SelectButtonWidth, position.y, c_SelectButtonWidth, EditorGUIUtility.singleLineHeight),
                    "*"))
                    selectKey();

                // current language
                var languages = localizationManager.GetLanguages();
                var selectedLanguage = EditorGUI.Popup(new Rect(position.x + position.width - c_LanguagePopupWidth, position.y, c_LanguagePopupWidth, EditorGUIUtility.singleLineHeight),
                    languages.IndexOf(localizationManager.Language), languages.ToArray());

                // change current localization
                if (selectedLanguage != -1 && languages[selectedLanguage] != localizationManager.Language)
                    localizationManager.Language = languages[selectedLanguage];

                // draw textField & update key
                updateValue(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight));
            }
            else
            {
                // draw text only version
                updateValue(new Rect(position.x + EditorGUIUtility.labelWidth, position.y, position.width - EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight));
            }

            /////////////////////////////////////
            void updateValue(Rect textRect)
            {
                if (hasKey)
                {
                    // update data
                    var value = localizationManager.Localize(key.stringValue);
                    var localizationValue = EditorGUI.TextArea(textRect, value);

                    // value was changed, update key value
                    if (localizationValue != value)
                        localizationManager.ChangeValue(key.stringValue, localizationValue);
                }
                else
                {
                    // show placeholder, if changed then create new key
                    var localizationValue = EditorGUI.TextArea(textRect, LocalizationManager.c_DefaultKeyValue);

                    // not default value, initialize key
                    if (localizationValue != LocalizationManager.c_DefaultKeyValue)
                        localizationManager.CreateKey(key.stringValue, localizationValue);
                }
            }

            void reloadDictionary()
            {
                // remove key, save changes
                localizationManager.Read(null, true);
            }

            void selectKey()
            {
                var keys = localizationManager.GetDictionary().Select(n => n.Key).ToList();
                keys.Sort(StringComparer.CurrentCulture);
                ObjectPickerWindow.Show(controlID, key.stringValue, keys, 0, s => new GUIContent(s), "Select localization key");
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var textFieldHeight = property.isExpanded ? 1 : 0;
            return EditorGUIUtility.singleLineHeight * (1 + textFieldHeight);
        }
    }
}