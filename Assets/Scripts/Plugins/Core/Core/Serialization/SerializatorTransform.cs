using System;
using Core.Module;
using UnityEngine;

namespace Core
{
    [Serializable]
    public class SerializatorTransform : MonoBehaviour, Serialization.IComponent
    {
        private const string c_KeyPosition		= "pos";
        private const string c_KeyRotation		= "rot";
        private const string c_KeyScale			= "scale";
	
        //////////////////////////////////////////////////////////////////////////
        public void iSave(Serialization.SerializationInfoWrapper info)
        {
            info.AddValue(c_KeyPosition, gameObject.transform.localPosition);
            info.AddValue(c_KeyRotation, gameObject.transform.localRotation);
            info.AddValue(c_KeyScale, gameObject.transform.localScale);
        }

        public void iLoad(Serialization.SerializationInfoWrapper info)
        {
            gameObject.transform.localPosition	= info.GetValue<Vector3>(c_KeyPosition);
            gameObject.transform.localRotation	= info.GetValue<Quaternion>(c_KeyRotation);
            gameObject.transform.localScale		= info.GetValue<Vector3>(c_KeyScale);
        }
    }
}