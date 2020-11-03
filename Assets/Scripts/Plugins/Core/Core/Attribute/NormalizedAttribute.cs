using UnityEngine;
using System;

namespace Core
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class NormalizedAttribute : PropertyAttribute
    {
        public Vector4	m_DefaultValue;
	
        //////////////////////////////////////////////////////////////////////////
        public NormalizedAttribute()
        {
            m_DefaultValue = default;
        }

        public NormalizedAttribute(float defaultX, float defaultY, float defaultZ, float defaultW)
        {
            this.m_DefaultValue = new Vector4(defaultX, defaultY, defaultZ, defaultW);
        }
    }
}