using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Core.Effect
{
    [RequireComponent(typeof(Animator))]
    [DefaultExecutionOrder(1)]
    public sealed class Effect : MonoBehaviour
    {
        public interface IModule
        {
            Effect Effect { get; set; }

            void Begin();
            void End();
        }

        //////////////////////////////////////////////////////////////////////////
        private Animator        m_Animator;
        private List<IModule>   m_Modules;
        private List<IModule>   Modules => m_Modules ?? (m_Modules = GetComponentsInChildren<IModule>(true).ToList());

        [SerializeField]
        private bool            m_AutoRun;
        [SerializeField]
        private float           m_Speed = 1.0f;

        private float           m_Duration;

        //////////////////////////////////////////////////////////////////////////
        private void Awake()
        {
            m_Animator = GetComponent<Animator>();
            m_Animator.speed = 0.0f;
            
            // activate modules
            foreach (var module in Modules)
                module.Effect = this;

            // auto run check
            if (m_AutoRun)
                StartCoroutine(Run());
        }

        public T GetModule<T>() where T : IModule
        {
            return (T)Modules.FirstOrDefault(n => n is T);
        }

        public Effect SetSpeed(float speed)
        {
            // remove speed influence
            m_Duration *= m_Speed;

            m_Speed = speed;
            m_Duration *= (1.0f / m_Speed);

            return this;
        }

        public IEnumerator Run(Action onComplete = null)
        {
            // un pause animator, get animation duration (it could be implemented from animator behaviour or animation event bo it requires more asset knowledge and work)
            m_Animator.speed = m_Speed;

            // activate modules
            foreach (var module in Modules)
                module.Begin();

            m_Duration = (m_Animator.GetCurrentAnimatorStateInfo(0).length + 0.05f) * (1.0f / m_Animator.speed);

            // wait end of animation
            while (m_Duration > 0.0f)
            {
                m_Duration -= Time.deltaTime;
                yield return null;
            }

            // deactivate modules
            foreach (var module in Modules)
                module.End();

            // call on complete
            onComplete?.Invoke();

            // release effect
            Destroy(gameObject);
        }

        [Button]
        public void Activate()
        {
            // un pause animator, get animation duration (it could be implemented from animator behaviour or animation event bo it requires more asset knowledge and work)
            m_Animator.speed = m_Speed;

            // activate modules
            foreach (var module in Modules)
                module.Begin();
        }

        [Button]
        public void Deactivate()
        {
            m_Animator.speed = 0.0f;

            // deactivate modules, release effect
            foreach (var module in Modules)
                module.End();
        }
    }

    public abstract class ModuleBase : MonoBehaviour, Effect.IModule
    {
        public Effect               Effect { get; set; }

        //////////////////////////////////////////////////////////////////////////
        public virtual void Begin() {}
        public virtual void End() {}
    }

    public abstract class ModuleUpdatable : ModuleBase
    {
        private bool                m_Run;

        //////////////////////////////////////////////////////////////////////////
        protected void Update()
        {
            if (m_Run)
                _Update();
        }
        
        protected abstract void _Update();

        public override void Begin()
        {
            m_Run = true;
        }

        public override void End()
        {
            m_Run = false;
        }
    }
}