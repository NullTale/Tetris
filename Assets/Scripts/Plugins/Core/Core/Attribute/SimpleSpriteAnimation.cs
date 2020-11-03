using UnityEngine;
using System.Collections;
using Malee;
using System;

namespace Core
{
    public class SimpleSpriteAnimation : MonoBehaviour
    {
        public float			m_FrameInterval = 1.0f;
        //public float			m_AnimationSpeed;
        [SerializeField]
        private UpdateMethod	m_UpdateMethod = UpdateMethod.FixedUpdate;
        public bool				m_Loop = true;
        public bool				m_RandomStartFrame = false;
        public float			m_UpdateInterval = 1.0f;
        private int				m_CurrentFrame = 0;
        private float				m_CurrentAnimationTime = 0;
        private float				m_AnimationLenght = 0;
        [SerializeField, Tooltip("if null whill be recived from current object")]
        private SpriteRenderer				m_AnimationTarget;
        [SerializeField, Reorderable]
#pragma warning disable 649
        private AnimationFramesArray		m_SpriteFrames;
#pragma warning restore 649
        private Coroutine					m_UpdateCoroutine;

        //////////////////////////////////////////////////////////////////////////
        [Serializable]
        public enum UpdateMethod
        {
            Render,
            FixedUpdate,
            Interval
        }

        [Serializable]
        public class AnimationFramesArray : ReorderableArray<Sprite>
        {
        }

        //////////////////////////////////////////////////////////////////////////
        private void Awake()
        {
            if (m_AnimationTarget == null)
                m_AnimationTarget = GetComponent<SpriteRenderer>();

            m_AnimationLenght = m_FrameInterval * m_SpriteFrames.Count;
        }

        private void OnDisable()
        {
            if (m_UpdateCoroutine != null)
            {
                StopCoroutine(m_UpdateCoroutine);
                m_UpdateCoroutine = null;
            }
        }

        private void OnEnable()
        {
            if (m_RandomStartFrame)
            {
                m_CurrentFrame = UnityEngine.Random.Range(0, m_SpriteFrames.Count);
                m_CurrentAnimationTime = m_FrameInterval * m_CurrentFrame;
            }

            if (m_AnimationTarget == null)
            {
                Debug.LogError($"{gameObject.name} SpriteAnimation target is null.");
                enabled = false;
                return;
            }

            switch (m_UpdateMethod)
            {
                case UpdateMethod.Render:
                    m_UpdateCoroutine = StartCoroutine(implUpdateCoroutine());
                    break;
                case UpdateMethod.FixedUpdate:
                    m_UpdateCoroutine = StartCoroutine(implFixedUpdateCoroutine());
                    break;
                case UpdateMethod.Interval:
                    m_UpdateCoroutine = StartCoroutine(implUpdateIntervalCoroutine());
                    break;
            }
        }


        //////////////////////////////////////////////////////////////////////////
        public IEnumerator implFixedUpdateCoroutine()
        {
            while (true)
            {
                if (m_CurrentAnimationTime >= m_AnimationLenght)
                {
                    if (m_Loop)
                    {	// reset timer
                        m_CurrentAnimationTime -= m_AnimationLenght;
                    }
                    else
                    {	// break animation
                        yield break;
                    }
                }

                m_CurrentFrame = Mathf.FloorToInt(m_CurrentAnimationTime / m_FrameInterval);
                m_AnimationTarget.sprite = m_SpriteFrames[m_CurrentFrame];

                m_CurrentAnimationTime += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }
        }
	
        public IEnumerator implUpdateCoroutine()
        {
            while (true)
            {
                if (m_CurrentAnimationTime >= m_AnimationLenght)
                {
                    if (m_Loop)
                    {	// reset timer
                        m_CurrentAnimationTime -= m_AnimationLenght;
                    }
                    else
                    {	// break animation
                        yield break;
                    }
                }

                m_CurrentFrame = Mathf.FloorToInt(m_CurrentAnimationTime / m_FrameInterval);
                m_AnimationTarget.sprite = m_SpriteFrames[m_CurrentFrame];

                m_CurrentAnimationTime += Time.deltaTime;
                yield return null;
            }
        }

        public IEnumerator implUpdateIntervalCoroutine()
        {
            while (true)
            {
                if (m_CurrentAnimationTime >= m_AnimationLenght)
                {
                    if (m_Loop)
                    {	// reset timer
                        m_CurrentAnimationTime -= m_AnimationLenght;
                    }
                    else
                    {	// break animation
                        yield break;
                    }
                }

                m_CurrentFrame = Mathf.FloorToInt(m_CurrentAnimationTime / m_FrameInterval);
                m_AnimationTarget.sprite = m_SpriteFrames[m_CurrentFrame];

                // buggy logic, no guarantee that m_UpdateInterval time has passed
                m_CurrentAnimationTime += m_UpdateInterval;
                yield return new WaitForSeconds(m_UpdateInterval);
            }
        }
    }
}