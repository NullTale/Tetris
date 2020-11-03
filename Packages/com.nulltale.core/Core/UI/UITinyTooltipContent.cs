using UnityEngine;
using UnityEngine.EventSystems;

namespace Core
{
    public class UITinyTooltipContent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [TextArea(4, 10)] 
        public string m_TooltipContent;

        //////////////////////////////////////////////////////////////////////////
        public void OnPointerEnter(PointerEventData eventData)
        {
            UITinyTooltip.Instance.Show(m_TooltipContent);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            UITinyTooltip.Instance.Hide();
        }
    }
}