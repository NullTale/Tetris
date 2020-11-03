using System;
using System.Collections.Generic;
using Malee;
using RotaryHeart.Lib.SerializableDictionary;
using UnityEngine;

namespace Core
{
    [Serializable, CreateAssetMenu]
    public class NameGenerator : ScriptableObject
    {
        [Serializable]
        public class SyllableDictionary : SerializableDictionaryBase<string, SyllableList> {};

        [Serializable]
        public class RulesDictionary : SerializableDictionaryBase<string, int> {};

        [Serializable]
        public class SyllableList
        {
            public string				m_Prefix;
            public string				m_Postfix;
            [Reorderable]
            public StringList			m_StringList;

            public string GetSyllable()
            {
                return m_Prefix + UnityRandom.RandomFromList(m_StringList) + m_Postfix;
            }
        }

        [Serializable]
        public class StringList : ReorderableArray<string> {}

        [Serializable]
        public class RulesList : ReorderableArray<RulesDictionary> {}

        public class NameElement
        {
            public string	m_Tag;
            public string	m_Syllable;

        }

        //////////////////////////////////////////////////////////////////////////
        [Reorderable]
        public RulesList				m_RulesList = new RulesList();

        public SyllableDictionary		m_SyllableDictionary;

        private string					m_LastName = "";
	
        //////////////////////////////////////////////////////////////////////////
        public string GetNextRandomName(List<string> blackList)
        {
            var result = "";
            var safeCounter = 100;
            do
            {	// roll until black list doesn't contain result
                result = GetNextRandomName();
            } while (blackList.Contains(result) && --safeCounter > 0);

            return result;
        }

        public string GetNextRandomName()
        {
            var result = "";

            // get random token combination
            var rule = UnityRandom.RandomFromList(m_RulesList);
            foreach (var token in rule)
            {
                // roll token chance
                if (UnityRandom.Bool(token.Value))
                {
                    var tokenChoise = m_SyllableDictionary[token.Key];
                    result += tokenChoise.GetSyllable();
                }
            }

            return result;
        }

        public string GetNextRandomName(ICollection<NameElement> tokens)
        {
            if (tokens == null)
                return GetNextRandomName();

            var result = "";
		
            var rule = UnityRandom.RandomFromList(m_RulesList);
            foreach (var token in rule)
            {
                if (UnityRandom.Bool(token.Value))
                {
                    var tokenChoise = m_SyllableDictionary[token.Key];
                    var syllable = tokenChoise.GetSyllable();
                    result += syllable;

                    tokens.Add(new NameElement(){m_Tag = token.Key, m_Syllable = syllable });
                }
            }

            return result;
        }

    }
}