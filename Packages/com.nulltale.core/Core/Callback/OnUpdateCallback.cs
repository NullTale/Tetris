using System;

namespace Core
{
    public class OnUpdateCallback : CallbackBase
    {
        public Action Action    {get; set;}
	
        //////////////////////////////////////////////////////////////////////////
        private void Update()
        {
            Action.Invoke();
        }
    }
}