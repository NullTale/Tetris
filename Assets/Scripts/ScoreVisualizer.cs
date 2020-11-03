using System;
using System.Collections;
using TMPro;
using UnityEngine;

[Serializable]
public class ScoreVisualizer : MonoBehaviour
{
    private static readonly int     Increment = Animator.StringToHash("Increment");

    public TMP_Text                 m_Text;
    public Animator                 m_Animator;

    private int                     m_Scores;
    private int                     m_ScoresTarget;
    private int                     m_ScoresInitial;

    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float                   m_ScoreTransition;

    public int                      Scores
    {
        set
        {
            m_ScoresInitial = m_Scores;
            m_ScoresTarget = value;

            m_Animator.SetTrigger(Increment);
        }
    }

    //////////////////////////////////////////////////////////////////////////
    private void Awake()
    {
        m_Text.text = m_Scores.ToString();
    }

    private void Update()
    {
        if (m_ScoresTarget != m_Scores)
        {
            m_Scores = Mathf.RoundToInt(Mathf.LerpUnclamped(m_ScoresInitial, m_ScoresTarget, m_ScoreTransition));
            m_Text.text = m_Scores.ToString();
        }
    }
}