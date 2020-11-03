using UnityEngine;

namespace Core
{
    public class CallbackBase : MonoBehaviour
    {
        //////////////////////////////////////////////////////////////////////////
        protected void Awake()
        {
            hideFlags = HideFlags.HideInInspector;
        }
    }
}