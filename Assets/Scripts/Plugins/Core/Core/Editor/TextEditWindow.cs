using System;
using UnityEditor;
using UnityEngine;

namespace Core
{
    internal sealed class TextEditWindow : EditorWindow
    {
        private static int              m_FieldID;
        private string                  m_Text;
        private Action<string>          m_OnOk;

        //////////////////////////////////////////////////////////////////////////
        public static void Show(int fieldID, string text, Action<string> onOk)
        {
            m_FieldID = fieldID;

            // configure window
            var window = CreateInstance<TextEditWindow>();
            window.titleContent = new GUIContent("Edit ");
            //window.maxSize = new Vector2(200, InternalGUI.SearchBarHeight + InternalGUI.LabelHeight);
            window.minSize = new Vector2(112, 21);
            window.maxSize = new Vector2(300, 21);
            window.m_Text = text;
            window.m_OnOk = onOk;

            // show
            window.ShowAuxWindow();
        }
    
        /*public static void TryGetPickedObject<T>(int fieldID, ref T picked)
        {
            if (_HasPickedObject && m_FieldID == fieldID)
            {
                picked = (T)_PickedObject;
                _PickedObject = null;
                _HasPickedObject = false;
                GUI.changed = true;
            }
        }*/

        private void OnGUI()
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
                case EventType.ValidateCommand:
                case EventType.ExecuteCommand:
                case EventType.ContextClick:
                    return;

                default:
                    break;
            }

            // consume commands
            if (CheckInput())
            {
                Event.current.Use();
                return;
            }
            
            // draw input field
            var area = new Rect(0, 0, position.width, position.height);
            DrawSearchBar(ref area);
        }
        
        private bool CheckInput()
        {
            var currentEvent = Event.current;
            if (currentEvent.type == EventType.KeyUp)
            {
                switch (currentEvent.keyCode)
                {
                    case KeyCode.KeypadEnter:
                    case KeyCode.Return:
                        m_OnOk?.Invoke(m_Text);
                        Close();
                        UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
                        return true;

                    case KeyCode.Escape:
                        Close();
                        return true;

                    default:
                        break;
                }
            }

            return false;
        }
        
        private void DrawSearchBar(ref Rect area)
        {
            area.height = InternalGUI.SearchBarHeight;
            GUI.BeginGroup(area, EditorStyles.toolbar);
            {
                area.x += 2;
                area.y += 2;

                GUI.SetNextControlName("SearchFilter");
                EditorGUI.BeginChangeCheck();
                m_Text = GUI.TextField(area, m_Text, GUI.skin.textArea);
                //if (EditorGUI.EndChangeCheck()) OnSearchTextChanged(searchText);
                EditorGUI.FocusTextInControl("SearchFilter");

                area.x = area.xMax;
                area.width = InternalGUI.SearchBarEndStyle.fixedWidth;
                /*if (HasSearchText)
                {
                    if (GUI.Button(area, "", InternalGUI.SearchBarCancelStyle))
                    {
                        m_Text = "";
                    }
                }
                else
                    GUI.Box(area, "", InternalGUI.SearchBarEndStyle);*/
            }
            GUI.EndGroup();

            area.x = 0;
            area.width = position.width;
            area.y += area.height;
        }
    }
}