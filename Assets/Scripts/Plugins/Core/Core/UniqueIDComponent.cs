using UnityEngine;

namespace Core
{
    [DefaultExecutionOrder(-100)]
    public sealed class UniqueIDComponent : MonoBehaviour, IUniqueID
    {

        [SerializeField, Tooltip("Useful with prefab instantiation")]
        private bool			m_GenerateOnStart;
        [SerializeField]
        private UniqueID		m_UniqueID;
	
        public string		ID
        {
            get => m_UniqueID.ID;
            set
            {
                m_UniqueID.ID = value;
            }
        }

        //////////////////////////////////////////////////////////////////////////
        private void Awake()
        {
            if (m_GenerateOnStart)
                m_UniqueID = new UniqueID();

        }

        public void GenerateGuid()
        {
            m_UniqueID.GenerateGuid();
        }
    }
}
#pragma warning disable 649