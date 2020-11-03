using System;
using System.Reflection;
using UnityEngine;

namespace Core
{
    [Serializable]
    public class TypeReference : ISerializationCallbackReceiver
    {
        private Type		m_Type;
        [SerializeField]
        private string		m_TypeName;

        public Type			Type => m_Type;

        //////////////////////////////////////////////////////////////////////////
        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
            if (!string.IsNullOrEmpty(m_TypeName)) 
            {
                m_Type = Type.GetType(m_TypeName);

                if (m_Type == null)
                    Debug.LogWarning(string.Format("'{0}' was referenced but class type was not found.", m_TypeName));
            }
            else 
            {
                m_Type = null;
            }
        }

        protected bool Equals(TypeReference other)
        {
            return m_TypeName == other.m_TypeName;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TypeReference) obj);
        }

        public override int GetHashCode()
        {
            return (m_TypeName != null ? m_TypeName.GetHashCode() : 0);
        }
    }
}