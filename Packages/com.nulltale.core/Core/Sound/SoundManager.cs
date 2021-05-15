using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.EventSystem;
using CsvHelper;
using Malee;
using NaughtyAttributes;
using RotaryHeart.Lib.SerializableDictionary;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;

namespace Core
{
    [Serializable]
    public enum SoundManagerCommand
    {
        PlayMusic,
        PlayAmbient,
        PlaySound,
    }

    [DefaultExecutionOrder(global::Core.Core.c_ManagerDefaultExecutionOrder)]
    public class SoundManager : MessageListener<SoundManagerCommand>, Core.IModule
    {

        [Serializable]
        public abstract class AudioDatabase : ScriptableObject
        {
            [SerializeField]
            private string                  m_Prefix;
            public string                   Prefix => m_Prefix;

            public abstract IEnumerable<KeyValuePair<string, IAudioData>> GetAudioData();
        }

        [Serializable]
        public abstract class AudioChannel
        {
            [SerializeField]
            private AudioSource         m_AudioSource;
            public AudioSource          AudioSource => m_AudioSource;

            //////////////////////////////////////////////////////////////////////////
            public virtual void Play(string audioDataKey)
            {
                Play(Instance.GetAudioData(audioDataKey));
            }

            protected abstract void Play(AudioData audioData);
        }

        [Serializable]
        public class MusicChannel : AudioChannel
        {
            [SerializeField]
            private AnimationCurve      m_Leave = new AnimationCurve(new Keyframe(0.0f, 1.0f), new Keyframe(1.2f, 0.0f));
            [SerializeField]
            private AnimationCurve      m_Enter = new AnimationCurve(new Keyframe(0.0f, 0.0f), new Keyframe(2.0f, 1.0f));

            private string              m_Playing;
            public string               Playing 
            { 
                get => m_Playing;
                set
                {
                    if (m_Playing == value)
                        return;

                    Play(value);
                }
            }

            //////////////////////////////////////////////////////////////////////////
            public override void Play(string audioDataKey)
            {
                var ad = Instance.GetAudioData(audioDataKey);
                m_Playing = ad == null ? null : audioDataKey;

                Play(ad);
            }

            protected override void Play(AudioData audioData)
            {
                if (AudioSource.clip == audioData?.Clip)
                    return;

                Instance.StartCoroutine(_TransitionCoroutine(AudioSource, audioData, m_Leave, m_Enter));
            }

            public void Stop()
            {
                Play((string)null);
            }
        }

        [Serializable]
        public class AmbientChannel : AudioChannel
        {
            [SerializeField]
            private AnimationCurve      m_Leave = new AnimationCurve(new Keyframe(0.0f, 1.0f), new Keyframe(2.2f, 0.0f));
            [SerializeField]
            private AnimationCurve      m_Enter = new AnimationCurve(new Keyframe(0.0f, 0.0f), new Keyframe(2.2f, 1.0f));

            private string              m_Playing;
            public string               Playing 
            { 
                get => m_Playing;
                set
                {
                    if (m_Playing == value)
                        return;

                    Play(value);
                }
            }

            //////////////////////////////////////////////////////////////////////////
            public override void Play(string audioDataKey)
            {
                var ad = Instance.GetAudioData(audioDataKey);
                m_Playing = ad == null ? null : audioDataKey;

                Play(ad);
            }

            protected override void Play(AudioData audioData)
            {
                Instance.StartCoroutine(_TransitionCoroutine(AudioSource, audioData, m_Leave, m_Enter));
            }

            public void Stop()
            {
                Play((string)null);
            }
        }

        [Serializable]
        public class SoundChannel : AudioChannel
        {
            //////////////////////////////////////////////////////////////////////////
            protected override void Play(AudioData audioData)
            {
                if (audioData != null)
                    AudioSource.PlayOneShot(audioData.Clip, audioData.Volume);
            }
        }

        [Serializable]
        public class AudioDataContainer
        {
            [SerializeField, SerializeReference, ClassReference]
            private IAudioData      m_AudioData;
            public IAudioData       AudioData => m_AudioData;
        }
        
        [Serializable]
        public class AudioDataEx : IAudioData
        {
            public Type      m_Type;

            public AudioData AudioData
            {
                get
                {
                    switch (m_Type)
                    {
                        case Type.First:
                            return m_Sounds.FirstOrDefault();
                        case Type.Random:
                            return m_Sounds.RandomItem();
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }

            public List<AudioData>  m_Sounds;

            //////////////////////////////////////////////////////////////////////////
            [Serializable]
            public enum Type
            {
                First,
                Random,
            }
        }

        public interface IAudioData
        {
            AudioData        AudioData { get; }
        }

        [Serializable]
        public class AudioData
        {
            [Range(0, 1)]
            public float        Volume;
            public AudioClip    Clip;
        }

        [Serializable]
        public class Clip : IAudioData
        {
            [Range(0.0f, 1.0f)]
            public float            m_Volume = 1.0f;
            public AudioClip        m_AudioClip;
        
            public AudioData        AudioData => new AudioData()
            {
                Clip = m_AudioClip,
                Volume = m_Volume
            };
        }

        [Serializable]
        public class ClipVolumeRange : IAudioData
        {
            [MinMaxSlider(0.0f, 1.0f)]
            public Vector2          m_VolumeRange = new Vector2(1.0f, 1.0f);
            public AudioClip        m_AudioClip;
        
            public AudioData        AudioData => new AudioData()
            {
                Clip = m_AudioClip,
                Volume = UnityEngine.Random.Range(m_VolumeRange.x, m_VolumeRange.y)
            };
        }

        [Serializable]
        public class Random : IAudioData
        {
            public bool                     m_DoNotRepeat;
            public List<Clip>               m_AudioList;
            private Clip                    m_Last;
            public AudioData                AudioData
            {
                get
                {
                    if (m_AudioList.Count == 0)
                        return null;

                    if (m_AudioList.Count == 1)
                        return m_AudioList[0].AudioData;

                    if (m_DoNotRepeat)
                    {
                        var result = m_AudioList.RandomItem(m_Last);
                        m_Last = result;
                        return result.AudioData;
                    }
                    else
                        return m_AudioList.RandomItem().AudioData;
                }
            }
        }

        //////////////////////////////////////////////////////////////////////////
        public static SoundManager                      Instance;
        public static MusicChannel                      Music => Instance.m_Music;
        public static AmbientChannel                    Ambient => Instance.m_Ambient;
        public static SoundChannel                      Sound => Instance.m_Sound;
        public static AudioMixer                        Mixer => Instance.m_Mixer;

        [SerializeField]
        private MusicChannel                            m_Music;
        [SerializeField]
        private AmbientChannel                          m_Ambient;
        [SerializeField]
        private SoundChannel                            m_Sound;

        [SerializeField]
        private AudioMixer                              m_Mixer;

        [SerializeField]
        private Dictionary<string, IAudioData>          m_AudioData;

        [SerializeField]
        [Reorderable(labels = false)]
        private ReorderableArrayT<AudioDatabase>        m_AudioDatabases;

        [SerializeField]
        private SerializableDictionaryBase<string, Clip>  m_EmbeddedAudioDatabase;

        //////////////////////////////////////////////////////////////////////////
        public void Init()
        {
            Instance = this;

            // allocate audio dictionary
            m_AudioData = new Dictionary<string, IAudioData>();

            // move audio sources to the main camera
            m_Music.AudioSource.transform.SetParent(Core.Instance.Camera.transform, false);
            m_Ambient.AudioSource.transform.SetParent(Core.Instance.Camera.transform, false);
            m_Sound.AudioSource.transform.SetParent(Core.Instance.Camera.transform, false);
            
            // add embedded database
            addDatabase(null, m_EmbeddedAudioDatabase.Select(n => new KeyValuePair<string, IAudioData>(n.Key, n.Value)));

            // add from prefabs
            foreach (var audioDatabase in m_AudioDatabases)
                if (audioDatabase != null)
                    addDatabase(audioDatabase.Prefix, audioDatabase.GetAudioData());

            // enable auto connection
            AutoConnect = true;

            /////////////////////////////////////
            void addDatabase(string prefix,  IEnumerable<KeyValuePair<string, IAudioData>> database)
            {
                if (string.IsNullOrWhiteSpace(prefix))
                    foreach (var contaner in database)
                    {
                        var data = contaner.Value;
                        if (data != null)
                            m_AudioData.Add(contaner.Key, data);
                    }
                else
                    foreach (var contaner in database)
                    {
                        var data = contaner.Value;
                        if (data != null)
                            m_AudioData.Add(prefix + contaner.Key, data);
                    }

            }
        }

        public AudioData GetAudioData(string sound)
        {
            if (sound != null && m_AudioData.TryGetValue(sound, out var audioData))
                return audioData?.AudioData;

            return null;
        }

        public bool GetAudioData(string sound, out AudioData audioData)
        {
            if (sound != null && m_AudioData.TryGetValue(sound, out var iAudioData))
            {
                audioData = iAudioData.AudioData;
                return true;
            }

            audioData = default;
            return false;
        }

        public IEnumerable<string> GetAudioDataKeys()
        {
            foreach (var audioDatabase in m_AudioDatabases.Where(n => n != null).Select(n => (n.Prefix, n.GetAudioData()))
                .Append((string.Empty, m_EmbeddedAudioDatabase.Select(n => new KeyValuePair<string, IAudioData>(n.Key, n.Value)))))
            foreach (var container in audioDatabase.Item2)
                yield return audioDatabase.Item1 + container.Key;
        }

        public AudioMixer GetMixer()
        {
            return m_Mixer;
        }

        //////////////////////////////////////////////////////////////////////////
        private static IEnumerator _TransitionCoroutine(AudioSource audioSource, AudioData audioData, AnimationCurve leave, AnimationCurve enter)
        {
            // run leave transition, if playing
            if (audioSource.isPlaying)
                yield return volumeCurve(leave, audioSource.volume);

            // run enter transition, if clip not null
            if (audioData?.Clip != null)
            {
                audioSource.clip = audioData.Clip;
                audioSource.Play();
                yield return volumeCurve(enter, audioData.Volume);
            }

            /////////////////////////////////////
            IEnumerator volumeCurve(AnimationCurve curve, float initialVolume)
            {
                var lastKey = curve.keys.Last();
                var duration = lastKey.time;
                for (var currentTime = 0.0f; currentTime < duration; currentTime += Time.deltaTime)
                {
                    audioSource.volume = initialVolume * curve.Evaluate(currentTime / duration);
                    yield return null;
                }

                // apply last key
                audioSource.volume = initialVolume * lastKey.value;
            }
        }

        public override void ProcessMessage(IMessage<SoundManagerCommand> e)
        {
            switch (e.Key)
            {
                case SoundManagerCommand.PlayMusic:
                    Music.Play(e.GetData<string>());
                    break;
                case SoundManagerCommand.PlayAmbient:
                    Ambient.Play(e.GetData<string>());
                    break;
                case SoundManagerCommand.PlaySound:
                    Sound.Play(e.GetData<string>());
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}