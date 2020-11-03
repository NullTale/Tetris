using System;
using System.IO;
using System.Runtime.Serialization;
using UnityEditor;
using UnityEngine;

namespace Core
{
    [Serializable]
    public abstract class SerializableScriptableObject : ScriptableObject, ISerializationCallbackReceiver
    {
        private const string		c_ResourcesPrefix = "Resources/";
        public string				m_ResourcePath;

        //////////////////////////////////////////////////////////////////////////
        public class SerializationSurrogate : ISerializationSurrogate
        {
            public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
            {
                info.AddValue("path", (obj as SerializableScriptableObject).m_ResourcePath);
            }
 
            public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
            {
                return Resources.Load(info.GetString("path"));
            }
        }

        //////////////////////////////////////////////////////////////////////////
        public virtual void OnBeforeSerialize()
        {
#if UNITY_EDITOR
            m_ResourcePath = AssetDatabase.GetAssetPath(this);;

            var indexOf = m_ResourcePath.IndexOf(c_ResourcesPrefix, StringComparison.Ordinal);
            m_ResourcePath = indexOf >= 0 ? 
                Path.ChangeExtension(m_ResourcePath.Substring(indexOf + c_ResourcesPrefix.Length), null) :
                "";
#endif
        }

        public virtual void OnAfterDeserialize()
        {
        }
    }
}

// the easiest way is to use json serialization using instanceID, but instanceID is not constant and will be different in final build, so you can't use data saved in editor