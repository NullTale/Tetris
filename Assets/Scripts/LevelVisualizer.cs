using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelVisualizer : MonoBehaviour
{
    private static readonly int     Increment = Animator.StringToHash("Increment");

    public int                      Level
    {
        set
        {
            m_Text.text = (value + 1).ToString();
            m_Animator.SetTrigger(Increment);
        }
    }
    public float                    Progress
    {
        set
        {
            m_Progeress.fillAmount = Mathf.Clamp01(value);
        }
    }
    public Animator                 m_Animator;
    public TMP_Text                 m_Text;
    public Image                    m_Progeress;
}
