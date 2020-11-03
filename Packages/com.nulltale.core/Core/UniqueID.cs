using System;
using UnityEngine;

namespace Core
{
    [Serializable]
    public class UniqueID : IUniqueID
    {
        // instead of string it could be bites or Guid
        [SerializeField]
        private string			m_GUID;

        public string			ID
        {
            get => m_GUID;
            set
            {
                m_GUID = value;
            }
        }

        //////////////////////////////////////////////////////////////////////////
        public UniqueID()
        {
            m_GUID = Guid.NewGuid().ToString();
        }

        public UniqueID(string id)
        {
            m_GUID = id;
        }

        public void GenerateGuid()
        {
            m_GUID = Guid.NewGuid().ToString();
        }

        public override bool Equals(object obj)
        {
            if (obj is UniqueID uid)
                return Equals(m_GUID, uid.m_GUID);
            if (obj is string str)
                return string.Equals(str, m_GUID);

            return false;
        }

        public override int GetHashCode()
        {
            return m_GUID.GetHashCode();
        }

        public override string ToString()
        {
            return m_GUID;
        }

        public static implicit operator string(UniqueID uid)
        {
            return uid.m_GUID;
        }
    }
}

public interface IUniqueID
{
	string ID { get; set; }
}