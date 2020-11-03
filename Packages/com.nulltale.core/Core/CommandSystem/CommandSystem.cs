using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace Core.CommandSystem
{
    public interface ICommand
    {
        IEnumerator Activate();
    }

    public abstract class CommandBase : ICommand
    {
        //////////////////////////////////////////////////////////////////////////
        public virtual IEnumerator Activate()
        {
            yield return Run();
        }

        protected abstract IEnumerator Run();
    }

    [Serializable]
    public class CommandCoroutine : CommandBase
    {
        [SerializeField]
        private CoroutineWrapper    m_Coroutine;

        protected override IEnumerator Run()
        {
            yield return _Listener();
        }

        private IEnumerator _Listener()
        {
            m_Coroutine.Start();
            while (m_Coroutine.IsRunning)
                yield return null;
        }
    }

    [Serializable]
    public class CommandAction : CommandBase
    {
        private Action    m_Action;

        public CommandAction(Action action)
        {
            m_Action = action;
        }

        protected override IEnumerator Run()
        {
            m_Action?.Invoke();
            yield break;
        }
    }

    [Serializable]
    public class CommandWait : CommandBase
    {
        [SerializeField]
        private float       m_Delay;
        public float        Delay
        {
            get => m_Delay;
            set => m_Delay = value;
        }

        //////////////////////////////////////////////////////////////////////////
        protected override IEnumerator Run()
        {
            yield return new WaitForSeconds(m_Delay);
        }
    }

    [Serializable]
    public class CommandLog : CommandBase
    {
        [SerializeField]
        private string    m_Log;

        public string Log
        {
            get => m_Log;
            set => m_Log = value;
        }

        protected override IEnumerator Run()
        {
            Debug.Log(m_Log);
            yield break;
        }
    }

    [Serializable]
    public class CommandNode : ICommand
    {
        [SerializeField, SerializeReference, ClassReference]
        private ICommand        m_Next;
        [SerializeField, SerializeReference, ClassReference]
        private ICommand        m_Current;

        //////////////////////////////////////////////////////////////////////////
        public IEnumerator Activate()
        {
            // activate this
            if (m_Current != null)
                yield return m_Current.Activate();

            // activate next
            if (m_Next != null)
                yield return m_Next.Activate();
        }
    }

    [Serializable]
    public class CommandSequence : ICommand
    {
        [SerializeField, SerializeReference, ClassReference]
        private List<ICommand>          m_Sequence = new List<ICommand>();
        
        //////////////////////////////////////////////////////////////////////////
        public IEnumerator Activate()
        {
            foreach (var command in m_Sequence)
                yield return command.Activate();
        }
    }
}