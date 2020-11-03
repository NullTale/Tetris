using System;
using System.Collections;
using Core;
using UnityEngine;

namespace Core
{
    public class ObjectSpawner : MonoBehaviour
    {
        [Serializable]
        public enum Mode
        {
            World,
            Self,
            Child,
        }

        //////////////////////////////////////////////////////////////////////////
        [SerializeField]
        private Mode                m_Mode;
        [SerializeField]
        [DrawIf(nameof(m_Mode), Mode.Child)]
        private Transform           m_Parent;

        //////////////////////////////////////////////////////////////////////////
        public void Spawn(GameObject go)
        {
            switch (m_Mode)
            {
                case Mode.World:
                    Instantiate(go, transform.position, Quaternion.identity);
                    break;
                case Mode.Self:
                    Instantiate(go, transform);
                    break;
                case Mode.Child:
                    Instantiate(go, m_Parent.transform);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
