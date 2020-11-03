using UnityEngine;
using UnityEngine.EventSystems;

namespace Core
{
    public class UITooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField]
        private GameObject		m_Target;

        //////////////////////////////////////////////////////////////////////////
        private void Awake()
        {
            if (m_Target == null)
                m_Target = gameObject;

            m_Target.SetActive(false);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            m_Target.SetActive(true);
        }


        public void OnPointerExit(PointerEventData eventData)
        {
            m_Target.SetActive(false);
        }
    }
}