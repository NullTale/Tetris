using System;
using UnityEngine;

namespace Core
{
    [Serializable]
    public class AnimatorParameterName
    {
        [SerializeField]
        private string       m_Name = "";
        [SerializeField]
        private int          m_Hash;

        public int          Hash => m_Hash;

        public string       Name
        {
            get => m_Name;
            set
            {
                // disallow null values
                m_Name = value ?? "";
                m_Hash = Animator.StringToHash(m_Name);
            }
        }
    
        //////////////////////////////////////////////////////////////////////////
        public AnimatorParameterName()
        {
            Name = null;
        }

        public AnimatorParameterName(string name)
        {
            Name = name;
        }

        public static implicit operator int(AnimatorParameterName apn)
        {
            return apn.Hash;
        }
    }
}