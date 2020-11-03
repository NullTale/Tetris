using System;
using UnityEngine;

namespace URandom
{
	[Serializable]
	public class WilledRandomInstance
	{
		[HideInInspector, NonSerialized]
		public float	m_RealChance;
		public bool		m_ResetOnProc = true;

	
		public float	m_ChanceMultyplyer = 1.0f;
		public float	m_ChanceIncrement = 0.0f;
	
		[HideInInspector, NonSerialized]
		public float			m_RollCount = 0.0f;
		public AnimationCurve	m_ChanceCurve = new AnimationCurve(new Keyframe(0.0f, 1.0f), new Keyframe(1.0f, 1.0f));
		
		[HideInInspector, NonSerialized]
		public UnityRandom		m_Generator;

		
		public bool Try()
		{
			m_RealChance = m_ChanceCurve.Evaluate(m_RollCount);
			m_RealChance *= m_ChanceMultyplyer * m_RollCount;
			m_RealChance += m_ChanceIncrement * m_RollCount;

			m_RollCount++;

			float chanceRoll = m_Generator != null ? m_Generator.RangeInclusive(0.0f, 1.0f) : UnityEngine.Random.value;

			if(chanceRoll <= m_RealChance)
			{	// succeeded
				if(m_ResetOnProc)
				{
					m_RollCount = 0.0f;
					m_RealChance = m_ChanceCurve.Evaluate(m_RollCount);
				}
				return true;
			}
			else
			{
				return false;
			}
		}
	}
}