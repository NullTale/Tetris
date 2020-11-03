using System;
using Core.Module;
using UnityEditor;
using UnityEngine;

namespace Core
{
    [Serializable]
    public class ResourcePathComponent : MonoBehaviour, ISerializationCallbackReceiver
    {
        [SerializeField]
        private string				m_ResourcePath;
        public string               ResourcePath => m_ResourcePath;

        //////////////////////////////////////////////////////////////////////////
        public void OnValidate()
        {
            _UpdateResourcePath();
        }

        public void OnBeforeSerialize()
        {
            _UpdateResourcePath();
        }

        public void OnAfterDeserialize()
        {
        }

        //////////////////////////////////////////////////////////////////////////
        private void _UpdateResourcePath()
        {
#if UNITY_EDITOR
            if (gameObject == null)
                return;

            var path = Serialization.AssetPathToResourcePath(
                PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(gameObject));
            if (string.IsNullOrEmpty(path) == false && m_ResourcePath != path)
            {
                m_ResourcePath = path;
                PrefabUtility.RecordPrefabInstancePropertyModifications(this);
            }
#endif
        }
    }
}