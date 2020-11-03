using System;
using NaughtyAttributes;
using UnityEngine;

namespace Core
{
    [Serializable]
    public class TweenTest : MonoBehaviour
    {
        [SerializeReference, ClassReference]
        public LeanTweenWrapper m_Tween;
        public LTWFollow    m_Follow;
        public LTWMove      m_Move;
        public LTWSequence  m_Sequence;

        //////////////////////////////////////////////////////////////////////////
        [Button]
        public void StartFollow()
        {
            m_Follow.Start();
        }
        [Button]
        public void StartMove()
        {
            m_Move.Start();
        }
        [Button]
        public void StartSequence()
        {
            m_Sequence.Start();
        }
    }
}