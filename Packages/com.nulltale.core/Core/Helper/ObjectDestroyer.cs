using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Core
{
    [Serializable]
    public class ObjectDestroyer : MonoBehaviour
    {
        [Serializable]
        public enum Method
        {
            Default,
            Immediate
        }

        //////////////////////////////////////////////////////////////////////////
        [SerializeField]
        private List<Object>        m_DestroyList;
        [SerializeField]
        private Method              m_Method;

        //////////////////////////////////////////////////////////////////////////
        public void DestroySelf()
        {
            _Destroy(gameObject);
        }

        public void DestroyList()
        {
            foreach (var obj in m_DestroyList)
                _Destroy(obj);
        }

        //////////////////////////////////////////////////////////////////////////
        private void _Destroy(Object go)
        {
            // object must exist
            if (go == null)
                return;

            // only in play mode
            if (Application.isPlaying == false)
                return;

            // implement
            switch (m_Method)
            {
                case Method.Default:
                    Destroy(go);
                    break;
                case Method.Immediate:
                    DestroyImmediate(go);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}