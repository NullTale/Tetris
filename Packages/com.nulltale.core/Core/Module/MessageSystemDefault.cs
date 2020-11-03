using UnityEngine;

namespace Core.Module
{
    [CreateAssetMenu(fileName = nameof(MessageSystemDefault), menuName = Core.c_CoreModuleMenu + nameof(MessageSystemDefault))]
    public class MessageSystemDefault : Core.Module
    {
        [SerializeField]
        private bool      m_CollectClasses;
        [SerializeField]
        private bool      m_CollectFunctions;

        //////////////////////////////////////////////////////////////////////////
        public override void Init()
        {
            // instantiate event manager game object
            var go = new GameObject(name);
            go.transform.SetParent(Core.Instance.transform, false);

            // do not call awake
            go.SetActive(false);
            var em = go.AddComponent<EventSystem.MessageSystemDefault>();

            // set parameters
            em.CollectClasses = m_CollectClasses;
            em.CollectFunctions = m_CollectFunctions;

            // initialize & activate go
            em.Init();
            go.SetActive(true);
        }
    }
}