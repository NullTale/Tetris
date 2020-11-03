using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Core.Module
{
    [CreateAssetMenu(fileName = nameof(FPSCounter), menuName = Core.c_CoreModuleMenu + nameof(FPSCounter))]
    public class FPSCounter : Core.Module
    {
        //////////////////////////////////////////////////////////////////////////
        [Range(0.1f, 60.0f)]
        public float                    c_FPSRange = 1.0f;

        private float                   m_FPS;
        private LinkedList<float>       m_FPSHistory = new LinkedList<float>();

        public float                    FPS => m_FPS;
        public string                   FPSText => m_FPS.ToString("##.00");


        [SerializeField]
        private bool                    m_Show;
        [SerializeField, DrawIf(nameof(m_Show), true)]
        private Rect                    m_GUIPosition = new Rect(16, 8, 50, 20);

        public bool Show
        {
            get => m_Show;
            set
            {
                m_Show = value;
                OnValidate();
            }
        }

        private OnGuiCallback           ShowCallback;

        //////////////////////////////////////////////////////////////////////////
        public void Update()
        {
            // add frame time
            m_FPSHistory.AddLast(Time.deltaTime);
            // get buffer observable time
            var sum = m_FPSHistory.Sum(n => n);
            // cut sum to observed range
            while (m_FPSHistory.Count != 0 && sum > c_FPSRange)
            {
                sum -= m_FPSHistory.First.Value;
                m_FPSHistory.RemoveFirst();
            }

            // count * (ratio / frame diapason)
            m_FPS = (m_FPSHistory.Count * (c_FPSRange / sum)) / c_FPSRange;
        }

        public void OnValidate()
        {
            // show if required
            if (m_Show && ShowCallback == null && Application.isPlaying)
            {
                ShowCallback = Core.Instance.gameObject.AddComponent<OnGuiCallback>();
                ShowCallback.Action = () =>
                {
                    GUI.Label(m_GUIPosition, FPSText);
                };
            }
            else if (ShowCallback != null)
            {
                Destroy(ShowCallback);
            }
        }

        public override void Init()
        {
            // update callback
            Core.Instance.gameObject.AddComponent<OnUpdateCallback>().Action = Update;

            // show callback
            if (m_Show)
            {
                ShowCallback = Core.Instance.gameObject.AddComponent<OnGuiCallback>();
                ShowCallback.Action = () =>
                {
                    GUI.Label(m_GUIPosition, FPSText);
                };
            }
        }
    }
}