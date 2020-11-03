using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.CommandSystem
{
    [Serializable]
    public sealed class CommandSequencer : MonoBehaviour
    {
        [SerializeField]
        private bool                    m_AutoRun;

        private LinkedList<ICommand>    m_Sequence = new LinkedList<ICommand>();
        private Coroutine               m_Coroutine;
        public bool                     IsActive => m_Coroutine != null;
        public bool                     IsRunning { get; private set; }

        public bool AutoRun
        {
            get => m_AutoRun;
            set
            {
                if (m_AutoRun == value)
                    return;

                m_AutoRun = value;

                // run if auto run
                if (m_AutoRun)
                    Run();

            }
        }

        //////////////////////////////////////////////////////////////////////////
        private class CommandEnumerator : ICommand
        {
            private IEnumerator      m_Enumerator;

            //////////////////////////////////////////////////////////////////////////
            public CommandEnumerator(IEnumerator enumerator)
            {
                m_Enumerator = enumerator;
            }

            public IEnumerator Activate()
            {
                yield return m_Enumerator;
            }
        }

        //////////////////////////////////////////////////////////////////////////
        private void OnEnable()
        {
            if (m_AutoRun)
                Run();
        }

        private void OnDisable()
        {
            Stop();
        }

        public void Push(ICommand command)
        {
            if (command == null)
                return;

            m_Sequence.AddLast(command);
        }
        
        public void Push(IEnumerator command)
        {
            if (command == null)
                return;

            m_Sequence.AddLast(new CommandEnumerator(command));
        }
        
        public void Push(Action command)
        {
            if (command == null)
                return;

            m_Sequence.AddLast(new CommandAction(command));
        }

        public bool Remove(ICommand command)
        {
            return m_Sequence.Remove(command);
        }

        public void Run()
        {
            if (m_Coroutine == null)
                m_Coroutine = StartCoroutine(_Activate());
        }

        public void Stop()
        {
            IsRunning = false;
            if (m_Coroutine != null)
            {
                StopCoroutine(m_Coroutine);
                m_Coroutine = null;
            }

        }

        public void Clear()
        {
            m_Sequence.Clear();
        }

        //////////////////////////////////////////////////////////////////////////
        private IEnumerator _Activate()
        {
            while (true)
            {
                // yield first command from list
                var next = m_Sequence.First;
                if (next != null)
                {
                    IsRunning = true;
                    m_Sequence.RemoveFirst();
                    yield return next.Value.Activate();
                }
                else
                {
                    // wait command
                    IsRunning = false;
                    yield return null;
                }
            }
        }

    }
}