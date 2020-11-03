using UnityEngine;

namespace Core.Effect
{
    public class SpawnModule : ModuleUpdatable
    {
        [SerializeField]
        private bool        m_Spawn;
        [SerializeField]
        private GameObject  m_Prefab;
        [SerializeField]
        private Transform   m_Parent;
        private GameObject  m_Instance;
        
        //////////////////////////////////////////////////////////////////////////
        public Effect Set(Transform parent = null, GameObject prefab = null)
        {
            // save values
            if (prefab != null)
                m_Prefab = prefab;

            if (parent != null)
                m_Parent = parent;

            return Effect;
        }

        public override void Begin()
        {
            // create instance
            m_Instance = Instantiate(m_Prefab, m_Parent);
            m_Instance.gameObject.SetActive(false);

            base.Begin();
        }

        public override void End()
        {
            Destroy(m_Instance);
            base.End();
        }

        protected override void _Update()
        {
            // update instance
            if (m_Spawn && m_Instance.gameObject.activeSelf == false)
            {
                m_Instance.SetActive(true);
            }
            else if (m_Instance.gameObject.activeSelf)
            {
                m_Instance.SetActive(false);
            }
        }
    }
}