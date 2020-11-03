using TMPro;
using UnityEngine;

namespace Core
{
    [DefaultExecutionOrder(-1)]
    public class UITinyTooltip : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text				m_Text;

        public static UITinyTooltip		Instance;

        //////////////////////////////////////////////////////////////////////////
        private void Awake()
        {
            Instance = this;
            gameObject.SetActive(false);
        }

        private void Update()
        {
            transform.position = global::Core.Core.Instance.MouseWorldPosition.ScreenPosition;
        }
	
        public void Show(string text)
        {
            gameObject.SetActive(true);
            m_Text.text = text;
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}