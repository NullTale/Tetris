using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Core
{
    [Serializable]
    public class HistoryBuffer<T>
    {
        protected int                   m_HistorySize;
        protected LinkedList<T>         m_Buffer;

        public LinkedList<T>            Buffer => m_Buffer;

        protected int                   HistorySize
        {
            get => m_HistorySize;
            set => m_HistorySize = Mathf.Max(value, 0);
        }

        //////////////////////////////////////////////////////////////////////////
        public virtual void AddExperiance(T experience)
        {
            // add history element
            m_Buffer.AddLast(experience);

            // remove excess
            while (m_Buffer.Count > m_HistorySize)
                m_Buffer.RemoveFirst();
        }

        public HistoryBuffer(int historySize, params T[] initialValue)
        {
            m_HistorySize = historySize;
            m_Buffer = new LinkedList<T>(initialValue);
        }
    }

    public class HistoryBufferInt : HistoryBuffer<int>
    {
        public int  Average { get; private set; }

        //////////////////////////////////////////////////////////////////////////
        public override void AddExperiance(int experience)
        {
            base.AddExperiance(experience);
            Average = (int)(Buffer.Sum() / (float)Buffer.Count);
        }

        public HistoryBufferInt(int historySize, params int[] initialValue) 
            : base(historySize, initialValue)
        {
            Average = (int)(Buffer.Sum() / (float)Buffer.Count);
        }
    }
}