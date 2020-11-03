using System;
using System.Collections.Generic;
using Malee;
using UnityEngine;

namespace Core
{
    [CreateAssetMenu(fileName = "RollCombination", menuName = "RollCombination")]
    public class RollCombination : ScriptableObject
    {
        [Reorderable]
        public PatternCondition m_PatternCondition;

        //////////////////////////////////////////////////////////////////////////
        [Serializable]
        public class Condition
        {
            public ConditionType	m_ConditionType;
            public RollValue		m_RollValue;

            //////////////////////////////////////////////////////////////////////////
            [Serializable]
            public enum ConditionType
            {
                Any,
                Empty,
                Exactly,
            }

            //////////////////////////////////////////////////////////////////////////
            public bool Check(RollValue rollValue)
            {
                switch (m_ConditionType)
                {
                    case ConditionType.Any:
                        return true;
                    case ConditionType.Empty:
                        return rollValue == RollValue.None;
                    case ConditionType.Exactly:
                        return rollValue == m_RollValue;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        [Serializable]
        public class Pattern
        {
            public string				m_Name;

            [Reorderable]
            public PatternList			m_PatternList;

            //////////////////////////////////////////////////////////////////////////
            [Serializable]
            public class PatternList : ReorderableArray<ConditionPattern> {}

            [Serializable]
            public class ConditionPattern
            {
                [Reorderable]
                public PlaceCondition	m_PlaceCondition;

                [Serializable]
                public class PlaceCondition : ReorderableArray<Condition> {}
            }
        }
	
        [Serializable]
        public class PatternCondition : ReorderableArray<Pattern> {}

        //////////////////////////////////////////////////////////////////////////
        public bool Check(List<RollValue> rolls, out Pattern pattern, out Pattern.ConditionPattern condition)
        {
            foreach (var pat in m_PatternCondition)
            {
                foreach (var cond in pat.m_PatternList)
                {
                    if (cond.m_PlaceCondition.Count == rolls.Count)
                    {
                        var validPattern = true;
                        for (var index = 0; index < rolls.Count; index++)
                        {
                            if (cond.m_PlaceCondition[index].Check(rolls[index]) == false)
                            {
                                validPattern = false;
                                break;
                            }
                        }
                        if (validPattern)
                        {
                            pattern = pat;
                            condition = cond;
                            return true;
                        }
                    }
                }
            }
		
            pattern = null;
            condition = null;
            return false;
        }
    }
}