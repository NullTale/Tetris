using System;

namespace Core
{
    public class OnGuiCallback : CallbackBase
    {
        public Action Action    {get; set;}
	
        //////////////////////////////////////////////////////////////////////////
        private void OnGUI()
        {
            Action.Invoke();
        }
    }
}