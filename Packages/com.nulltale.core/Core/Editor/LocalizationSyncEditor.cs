using UnityEditor;
using UnityEngine;

namespace Core
{
    /// <summary>
    /// Adds "Sync" button to LocalizationSync script.
    /// </summary>
    [CustomEditor(typeof(LocalizationSync))]
    public class LocalizationSyncEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var component = (LocalizationSync) target;

            if (GUILayout.Button("Sync"))
            {
                component.Sync();
            }
            if (GUILayout.Button("Read"))
            {
                component.Read();
            }
        }
    }
}