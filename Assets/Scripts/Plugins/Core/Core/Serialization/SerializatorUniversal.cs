using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Core.Module;
using UnityEngine;

namespace Core
{
    public class SerializatorUniversal : MonoBehaviour, Serialization.IComponent
    {
        [Serializable]
        public class ComponentData
        {
            public string	m_Type;
            public Dictionary<string, object>	m_FieldDictionary = new Dictionary<string, object>();
        }

        //////////////////////////////////////////////////////////////////////////
        public void iSave(Serialization.SerializationInfoWrapper info)
        {
            // get all components, serialize thous fields
            var componentDataList = new List<ComponentData>();
            var componentList = new List<Component>();
            gameObject.GetComponents(componentList);

            foreach (var serializableComponent in componentList
                .Where(n => 
                    n is Serialization.IComponent == false 
                    && n is SerializableObject == false
                    && n is UniqueIDComponent == false))
            {
                var type = serializableComponent.GetType();
                var data = new ComponentData(){ m_Type = type.FullName};
                componentDataList.Add(data);

                info.Prefix += type.FullName;
                foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.Instance))
                {
                    //field.Attributes SerializeField
                    //field.IsNotSerialized
                    data.m_FieldDictionary[field.Name] = field.GetValue(serializableComponent);
                }
                info.Prefix = info.Prefix.Remove(info.Prefix.Length - type.FullName.Length);
            }
            //componentDataList.Select(n => n.m_Type)
            info.AddValue("componentList", componentDataList);
        }

        public void iLoad(Serialization.SerializationInfoWrapper info)
        {
            // unpack data, remove undeclared components
            var typeList = info.GetValue<List<ComponentData>>("componentList");

            foreach (var componentData in typeList)
            {
                var type = Type.GetType(componentData.m_Type);

                if (type == null)
                    continue;

                var component = gameObject.GetComponent(type);
                if (component == null)
                    component = gameObject.AddComponent(type);

                foreach (var field in componentData.m_FieldDictionary)
                {
                    type.GetField(field.Key, BindingFlags.Public | BindingFlags.Instance).SetValue(component, field.Value);
                }
            }
        }
    }
}