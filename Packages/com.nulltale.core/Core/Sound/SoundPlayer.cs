using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public class SoundPlayer : MonoBehaviour
    {
        [Serializable]
        public enum Mode
        {
            AudioSource,
            SoundManager,
        }

        //////////////////////////////////////////////////////////////////////////
        [SerializeField]
        private Mode                    m_Mode = Mode.SoundManager;
        [SerializeField]
        [DrawIf(nameof(m_Mode), Mode.AudioSource)]
        private AudioSource             m_AudioSource;
        private float                   m_InitialVolume;

        //////////////////////////////////////////////////////////////////////////
        private void Awake()
        {
            if (m_Mode == Mode.AudioSource)
                m_InitialVolume = m_AudioSource.volume;
        }

        public void PlayClip(AudioClip clip)
        {
            switch (m_Mode)
            {
                case Mode.AudioSource:
                    m_AudioSource.clip = clip;
                    m_AudioSource.Play();
                    break;
                case Mode.SoundManager:
                    // play in sound manager
                    SoundManager.Sound.AudioSource.PlayOneShot(clip);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Play(string audioDataKey)
        {
            switch (m_Mode)
            {
                case Mode.AudioSource:
                    if (SoundManager.Instance.GetAudioData(audioDataKey, out var audioData))
                    {
                        // play in audioSource
                        m_AudioSource.volume = m_InitialVolume * audioData.Volume;
                        m_AudioSource.clip = audioData.Clip;
                        m_AudioSource.Play();
                    }
                    break;
                case Mode.SoundManager:
                    // play in sound manager
                    SoundManager.Sound.Play(audioDataKey);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Stop()
        {
            m_AudioSource.Stop();
        }

        public void Pause()
        {
            m_AudioSource.Pause();
        }
    }
}