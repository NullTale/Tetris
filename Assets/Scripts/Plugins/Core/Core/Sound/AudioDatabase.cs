using System;
using System.Collections.Generic;
using RotaryHeart.Lib.SerializableDictionary;
using UnityEngine;

namespace Core
{
    [Serializable]
    [CreateAssetMenu(fileName = "Audio Database", menuName = "Sound Manager/Audio Database")]
    public class AudioDatabase : SoundManager.AudioDatabase
    {
        [SerializeField]
        private SerializableDictionaryBase<string, SoundManager.Clip>  m_AudioData;
        public SerializableDictionaryBase<string, SoundManager.Clip>   AudioData => m_AudioData;

        //////////////////////////////////////////////////////////////////////////
        public override IEnumerable<KeyValuePair<string, SoundManager.IAudioData>> GetAudioData()
        {
            foreach (var container in m_AudioData)
                if (container.Value != null)
                    yield return new KeyValuePair<string, SoundManager.IAudioData>(container.Key, container.Value);
        }
    }
}