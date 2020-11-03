using System;
using System.Collections;
using System.Collections.Generic;
using CielaSpike;
using UnityEngine;

namespace Core.Module
{
    [CreateAssetMenu(fileName = nameof(TaskManager), menuName = Core.c_CoreModuleMenu + nameof(TaskManager))]
    public class TaskManager : Core.Module
    {
        [Serializable]
        public enum TaskManagerTaskCount
        {
            /// <summary>Cores count</summary>
            ProcessorX1,
            /// <summary>Cores count * 2</summary>
            ProcessorX2,
            /// <summary>Cores count + 2</summary>
            ProcessorN2,
            /// <summary>Cores count + 4</summary>
            ProcessorN4,
            /// <summary>Cores count + 8</summary>
            ProcessorN8,
            /// <summary>8 tasks limit</summary>
            N8,
            /// <summary>16 tasks limit</summary>
            N16,
            /// <summary>Unlimited</summary>
            Unlimited,
        }
        
        //////////////////////////////////////////////////////////////////////////
        public static TaskManager           Instance;

        private int						    m_ProcessorCount = 4;
        private LinkedList<Task>            m_TaskList = new LinkedList<Task>();
        private LinkedList<IEnumerator>     m_TaskQueue = new LinkedList<IEnumerator>();
        public TaskManagerTaskCount		    m_TaskManagerTaskCount = TaskManagerTaskCount.ProcessorN2;

        //////////////////////////////////////////////////////////////////////////
        public override void Init()
        {
            // set instance
            Instance = this;

            // init
            switch (m_TaskManagerTaskCount)
            {
                case TaskManagerTaskCount.ProcessorX1:
                    m_ProcessorCount = SystemInfo.processorCount;
                    break;
                case TaskManagerTaskCount.ProcessorX2:
                    m_ProcessorCount = SystemInfo.processorCount * 2;
                    break;
                case TaskManagerTaskCount.ProcessorN2:
                    m_ProcessorCount = SystemInfo.processorCount + 2;
                    break;
                case TaskManagerTaskCount.ProcessorN4:
                    m_ProcessorCount = SystemInfo.processorCount + 4;
                    break;
                case TaskManagerTaskCount.ProcessorN8:
                    m_ProcessorCount = SystemInfo.processorCount + 8;
                    break;
                case TaskManagerTaskCount.N8:
                    m_ProcessorCount = 8;
                    break;
                case TaskManagerTaskCount.N16:
                    m_ProcessorCount = 16;
                    break;
                case TaskManagerTaskCount.Unlimited:
                    m_ProcessorCount = Int32.MaxValue;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Update()
        {
            // clear completed tasks
            var current = m_TaskList.First;
		    
            while (current.Next != null)
            {
                if (current.Next.Value.State == TaskState.Done)
                {
                    m_TaskList.Remove(current.Next);
                }
			    
                current = current.Next;
                if (current == null)
                    break;
            }

            // start next
            while (m_TaskList.Count <= m_ProcessorCount && m_TaskQueue.Count != 0)
            {
                Core.Instance.StartCoroutineAsync(m_TaskQueue.First.Value, out var task);
                m_TaskList.AddLast(new LinkedListNode<Task>(task));
                m_TaskQueue.RemoveFirst();
            }
        }

        public void AddTask(IEnumerator task)
        {
            m_TaskQueue.AddLast(task);
        }
    }
}