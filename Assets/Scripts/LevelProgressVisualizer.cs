using System;
using Core.EventSystem;
using NaughtyAttributes;
using UnityEngine;

[Serializable]
public class LevelProgressVisualizer : MessageListener<BoardEvent>
{
    //////////////////////////////////////////////////////////////////////////
    [SerializeField] [CurveRange(0, 0, 20, 1)]
    private AnimationCurve                                      m_TransitionCurve;
    [SerializeField]
    private float                                               m_StepDuration;
    [SerializeField]
    private Animator                                            m_Sun;
    [SerializeField] [AnimatorParam(nameof(m_Sun))]
    private int                                                 m_SunBlend;
    [SerializeField]
    private Animator                                            m_Clouds;
    [SerializeField] [AnimatorParam(nameof(m_Clouds))]
    private int                                                 m_CloudsBlend;
    [SerializeField]
    private PostPrecessControl                                  m_CameraPostProcess;

    [SerializeField]
    private TransitionFrame                                     m_Start;
    [SerializeField]
    private TransitionFrame                                     m_Finish;
    
    private float                                               m_CurvePosition;
    //////////////////////////////////////////////////////////////////////////
    [Serializable]
    public class TransitionFrame
    {
        public float        Blend;
        public float        Contrast;
        public float        HueShift;

        public (float, float, float) Lerp(TransitionFrame frame, float scale)
        {
            return (Mathf.LerpUnclamped(Blend, frame.Blend, scale), Mathf.LerpUnclamped(Contrast, frame.Contrast, scale), Mathf.LerpUnclamped(HueShift, frame.HueShift, scale));
        }
    }

    //private float                                               
    //////////////////////////////////////////////////////////////////////////
    public override void ProcessMessage(IMessage<BoardEvent> e)
    {
        // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
        switch (e.Key)
        {
            case BoardEvent.Level:
            {
                // start transition
                LeanTween.value(gameObject, (t) =>
                {
                    // save curve pos
                    m_CurvePosition = t;

                    // get transition value
                    var scale = m_TransitionCurve.Evaluate(t);
                    var (blend, contrast, hue) = m_Start.Lerp(m_Finish, scale);

                    // apply to sun
                    m_Sun.SetFloat(m_SunBlend, blend);
                    // apply to clouds
                    m_Clouds.SetFloat(m_CloudsBlend, blend);

                    // apply to post process
                    m_CameraPostProcess.Contrast = contrast;
                    m_CameraPostProcess.HueShift = hue;

                }, m_CurvePosition, TetrisManager.Instance.LevelCounter.Level, Mathf.Abs(m_CurvePosition - (float)TetrisManager.Instance.LevelCounter.Level) * m_StepDuration);

            } break;
        }
    }
}