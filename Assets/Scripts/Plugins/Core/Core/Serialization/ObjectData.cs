using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Core.Module;

namespace Core
{
    [Serializable]
    public class ObjectData
    {
        [Serializable]
        public class FieldInfoData
        {
            public string       Name;
            public string       Data;
        }
        public List<FieldInfoData>       m_FieldData;

        //////////////////////////////////////////////////////////////////////////
        public static ObjectData Create(object obj)
        {
            var result = new ObjectData();

            result.Read(obj);

            return result;
        }

        public void Read(object obj)
        {
            // serialize fields
            m_FieldData = obj
                .GetType()
                .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
                .Where(n => n.GetCustomAttributes(typeof(SaveFieldAttribute), true).Any())
                .Select(fieldInfo => new FieldInfoData
                {
                    Name = fieldInfo.Name,
                    Data = Serialization.ObjectToString(fieldInfo.GetValue(obj))
                })
                .ToList();
        }

        public void Write(object obj)
        {
            var fields = obj
                .GetType()
                .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
                .ToDictionary(key => key.Name, value => value);

            // assign serialized fields, to object fields
            foreach (var fieldData in m_FieldData)
            {
                if (fields.TryGetValue(fieldData.Name, out var fieldInfo))
                    fieldInfo.SetValue(obj, Serialization.ObjectFromString(fieldInfo.FieldType, fieldData.Data));
            }
        }
    }
}