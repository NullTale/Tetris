using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Core.Module;
using UnityEngine;

namespace Core
{
    [Serializable, RequireComponent(typeof(UniqueIDComponent), typeof(ResourcePathComponent))]
    public sealed class SerializableObject : MonoBehaviour
    {
        private ResourcePathComponent   m_ResourcePath;
        private UniqueIDComponent	    m_UniqueID;
        public string				    ID => m_UniqueID.ID;
        public string                   ResourcePath => m_ResourcePath.ResourcePath;

        //////////////////////////////////////////////////////////////////////////
        public class SerializationSurrogate : ISerializationSurrogate
        {
            public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
            {
                if (obj is SerializableObject so)
                {
                    // save component list
                    so._GetSerializableComponents(out var componentPath, out var componentList);
                    info.AddValue("componentList", componentPath);
                    info.AddValue("guid", so.m_UniqueID.ID);
                    info.AddValue("resourcePath", so.m_ResourcePath);

                    // save component data
                    var infoWrapper = new Serialization.SerializationInfoWrapper(info);

                    for (var n = 0; n < componentList.Count; n++)
                    {
                        var path = componentPath[n];
                        infoWrapper.Prefix = path.InfoPrefix;
                        componentList[n].iSave(infoWrapper);
                    }
                }
            }
 
            public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
            {
                // find the object by guid if not presented in the scene create one
                var guid = info.GetString("guid");
                var so = Serialization.Instance.GetSerializableObject(guid);
                if (so == null)
                {
                    var resourcePath = info.GetString("resourcePath");
                    if (string.IsNullOrEmpty(resourcePath))
                        return null;
                    var prefab = Resources.Load(resourcePath);
                    if (prefab == null)
                        return null;
                    so = (Instantiate(prefab) as GameObject)?.GetComponent<SerializableObject>();
                }

                if (so == null)
                    // create new object
                    return null;

                so.m_UniqueID.ID = guid;

                // validate components
                var componentList = info.GetValue("componentList", typeof(List<ComponentPath>)) as List<ComponentPath>;
                var infoWrapper = new Serialization.SerializationInfoWrapper(info);
                foreach (var n in componentList)
                {
                    so._ValidateComponent(n, infoWrapper);
                }

                return so;
            }
        }

        [Serializable]
        public struct ComponentPath
        {
            // child index path
            //public List<int>		m_Path;
            // name path
            public string			m_Path;
            // type of serializable component
            public string			m_Type;

            public string			InfoPrefix => m_Path + "/" + m_Type;
        }

        //////////////////////////////////////////////////////////////////////////
        private void Awake()
        {
            m_UniqueID = GetComponent<UniqueIDComponent>();
            m_ResourcePath = GetComponent<ResourcePathComponent>();
            Serialization.Instance?.AddSerializableObject(this);
        }

        private void OnDestroy()
        {
            Serialization.Instance?.RemoveSerializableObject(this);
        }

        //////////////////////////////////////////////////////////////////////////
        private void _ValidateComponent(ComponentPath componentPath, Serialization.SerializationInfoWrapper info)
        {
            info.Prefix = componentPath.InfoPrefix;
            _FindGameObjectComponent(componentPath.m_Path, Type.GetType(componentPath.m_Type), out var component);
            (component as Serialization.IComponent)?.iLoad(info);
        }

        private void _FindGameObjectComponent(string path, Type type, out Component component)
        {
            var result = gameObject;
            // parse hierarchy
            foreach (var n in path.Split('/'))
            {
                var findResult = result.transform.Find(n);
                if (findResult == null)
                {	// if none create
                    var go = new GameObject(n);
                    go.transform.parent = result.transform;
                    findResult = go.transform;
                }
                result = findResult.gameObject;
            }

            // get or create component
            if (result.TryGetComponent(type, out component) == false)
                component = result.AddComponent(type);
        }

        private void _GetSerializableComponents(out List<ComponentPath> componentPath, out List<Serialization.IComponent> componentList)
        {
            componentList = GetComponentsInChildren<Serialization.IComponent>(true).ToList();
            componentPath = componentList
                .Select(n => new ComponentPath(){ m_Path = _GetComponentPath(n as Component), m_Type = n.GetType().FullName })
                .ToList();
        }
	
        private List<Serialization.IComponent> _RequireComponents(List<ComponentPath> componentList)
        {
            // validate or create components
            var result = new List<Serialization.IComponent>(componentList.Count);

            return result;
        }

        private string _GetComponentPath(Component component)
        {
            var current = component.transform;
            var result = "";

            while (current != transform)
            {
                result = current.name + "/" + result;
                current = current.parent;
            }

            return result.StartsWith("/") ? result.Substring(1) : result;
        }
    }
}