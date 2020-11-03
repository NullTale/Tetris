using UnityEngine;
using System;
using System.Collections.Generic;

namespace Core
{
    /// <summary>
    /// Draws the field/property ONLY if the compared property compared by the comparison type with the value of comparedValue returns true.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
    public class DrawIfAttribute : PropertyAttribute
    {
        public List<CompareData>	m_CompareList;
        public DisablingType		m_DisablingType;

        //////////////////////////////////////////////////////////////////////////
        public struct CompareData
        {
            public string m_PropertyName;
            public object m_PropertyValue;

            public CompareData(string propertyName, object propertyValue)
            {
                m_PropertyName = propertyName;
                m_PropertyValue = propertyValue;
            }
        }
        /// <summary>
        /// Types of comperisons.
        /// </summary>
        public enum DisablingType
        {
            ReadOnly = 2,
            DontDraw = 3
        }

        public enum CompateMethod
        {
            Equals,
            NotEquals
        }

        /// <summary>
        /// Only draws the field only if a condition is met. Supports enum and bools.
        /// </summary>
        /// <param name="comparedPropertyName">The name of the property that is being compared (case sensitive).</param>
        /// <param name="comparedValue">The value the property is being compared to.</param>
        /// <param name="disablingType">The type of disabling that should happen if the condition is NOT met. Defaulted to DisablingType.DontDraw.</param>
        public DrawIfAttribute(string comparedPropertyName, object comparedValue, DisablingType disablingType = DisablingType.DontDraw)
        {
            m_CompareList = new List<CompareData>(){ new CompareData(comparedPropertyName, comparedValue) };
            m_DisablingType = disablingType;
        }
	
        public DrawIfAttribute(string comparedPropertyName, DisablingType disablingType, params object[] comparedValue)
        {
            m_CompareList = new List<CompareData>(comparedValue.Length);
            foreach (var value in comparedValue)
                m_CompareList.Add(new CompareData(comparedPropertyName, value));

            m_DisablingType = disablingType;
        }

        public DrawIfAttribute(DisablingType disablingType, params CompareData[] comparedValue)
        {
            m_CompareList = new List<CompareData>(comparedValue);
            m_DisablingType = disablingType;
        }

        public DrawIfAttribute(string comparedPropertyName, DisablingType disablingType = DisablingType.DontDraw)
        {
            m_CompareList = new List<CompareData>(){ new CompareData(comparedPropertyName, null) };
            m_DisablingType = disablingType;
        }
    }
}