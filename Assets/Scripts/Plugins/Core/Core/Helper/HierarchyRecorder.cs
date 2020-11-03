#if UNITY_EDITOR 
using NaughtyAttributes;
using UnityEngine;
using UnityEditor.Animations;

namespace Core
{
    [ExecuteInEditMode]
    public class HierarchyRecorder : MonoBehaviour
    {
        public AnimationClip		m_Clip;
 
        public bool					m_Record = false;
        public float				m_SnaphotTime = 1.0f;
 
        private GameObjectRecorder	m_Recorder;

        //////////////////////////////////////////////////////////////////////////
        void Start()
        {
            // Create the GameObjectRecorder.
            m_Recorder = new GameObjectRecorder(gameObject);
 
            // Set it up to record the transforms recursively.
            m_Recorder.BindComponentsOfType<Transform>(gameObject, true);
        }

        void LateUpdate()
        {
            if (m_Clip == null)
                return;
	 
            if(m_Record)
            {
                m_Recorder.TakeSnapshot(Time.deltaTime);
            }
            else if(m_Recorder.isRecording)
            {
                m_Recorder.SaveToClip(m_Clip);
                m_Recorder.ResetRecording();
            }
        }

        [Button]
        void TakeSnapshot()
        {
            m_Recorder.TakeSnapshot(m_SnaphotTime);
        }
        [Button]
        void SaveClip()
        {
            m_Recorder.SaveToClip(m_Clip);
            m_Recorder.ResetRecording();
        }
    }
}
#endif