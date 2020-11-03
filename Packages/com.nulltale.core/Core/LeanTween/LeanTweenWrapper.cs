using System;

namespace Core
{
    [Serializable]
    public abstract class LeanTweenWrapper
    {
        protected LTDescr           m_Descriptor;
        public abstract LTDescr     Descriptor {get;}

        public abstract LTDescr Start();
        public abstract LTDescr Pause();
        public abstract void    Cancel();

        //////////////////////////////////////////////////////////////////////////
        public void Restart()
        {
            Cancel();
            Start();
        }
    }
}