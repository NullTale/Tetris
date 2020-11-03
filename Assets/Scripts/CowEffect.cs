using System;
using System.Collections;
using System.Collections.Generic;
using Core;
using NaughtyAttributes;
using UnityEngine;

public class CowEffect : MonoBehaviour
{
    [SerializeField]
    private AudioClip       m_PolishCowClip;
    [SerializeField]
    private Animator        m_Animator;

    [SerializeField]
    [AnimatorParam(nameof(m_Animator), AnimatorControllerParameterType.Bool)]
    private int             m_AnimatorParameter;
    private bool            m_ActiveLast;

    //////////////////////////////////////////////////////////////////////////
    private void FixedUpdate()
    {
        var active = SoundManager.Music.AudioSource.clip == m_PolishCowClip;

        if (m_ActiveLast != active)
            m_Animator.SetBool(m_AnimatorParameter, active);

        m_ActiveLast = active;

    }
}
