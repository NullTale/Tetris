using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using Core;
using NaughtyAttributes;
using UnityEngine;

namespace Core.Module
{
    [CreateAssetMenu(fileName = nameof(Serialization), menuName = Core.c_CoreModuleMenu + nameof(Serialization))]
    public class Serialization : Core.Module
    {
	    [Serializable]
	    public enum SaveMode
	    {
		    Scene,
		    Custom,
	    }

	    [Serializable]
	    public enum LoadMode
	    {
		    Default,
		    DestroyUnmodified
	    }
	    
	    public class SerializationInfoWrapper
	    {
		    private string					m_Prefix;
		    private SerializationInfo		m_Info;

		    public string Prefix
		    {
			    get => m_Prefix;
			    set => m_Prefix = value;
		    }

            //////////////////////////////////////////////////////////////////////////
		    public void AddValue<T>(string name, T data)
		    {
			    if (data == null)
				    m_Info.AddValue(m_Prefix + name, null);
			    else
				    m_Info.AddValue(m_Prefix + name, data, data.GetType());
		    }

		    /*public void AddValue(string name, object data, Type type)
		    {
			    if (data == null)
				    m_Info.AddValue(m_Prefix + name, null);
			    else
				    m_Info.AddValue(m_Prefix + name, data, type);
		    }*/
		    
		    public T GetValue<T>(string name)
		    {
			    return (T)m_Info.GetValue(m_Prefix + name,typeof(T));
		    }

		    public SerializationInfoWrapper(SerializationInfo info)
		    {
			    m_Info = info;
		    }

	    }

	    public interface IComponent
	    {
		    void iSave(SerializationInfoWrapper info);
		    void iLoad(SerializationInfoWrapper info);
	    }

	    //////////////////////////////////////////////////////////////////////////
	    public static Serialization	            Instance;
        private static Dictionary<Type, XmlSerializer>  c_XmlSerializers = new Dictionary<Type, XmlSerializer>();

	    private const string				            c_ResourcesPrefix = "Resources/";

	    public SaveMode						            m_SaveMode = SaveMode.Scene;
	    public LoadMode						            m_LoadMode = LoadMode.Default;
	    public string						            m_SavePath;
	    private SurrogateSelector			            m_SurrogateSelector;
	    private BinaryFormatter				            m_BinaryFormatter;

	    [Tooltip("SaveMode.Custom objects to save")]
	    public List<SerializableObject>		            m_CustomSave;
	    private HashSet<SerializableObject>	            m_SerializableObjects = new HashSet<SerializableObject>();

	    //////////////////////////////////////////////////////////////////////////
        public override void Init()
	    {
		    if (string.IsNullOrEmpty(m_SavePath))
			    m_SavePath = Application.persistentDataPath;

		    Instance = this;

		    // register surrogates, allocate binaryFormatter 
		    m_SurrogateSelector = new SurrogateSelector();
		    UnityCommon_Serializator.Reg(m_SurrogateSelector);

		    var streamingContext = new StreamingContext(StreamingContextStates.All);
		    var ssos = new SerializableScriptableObject.SerializationSurrogate();

		    m_SurrogateSelector.AddSurrogate(typeof(SerializableObject), streamingContext, new SerializableObject.SerializationSurrogate());
		    
		    foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
		    {
			    if (typeof(SerializableScriptableObject).IsAssignableFrom(type))
				    m_SurrogateSelector.AddSurrogate(type, streamingContext, ssos);
		    }

		    m_BinaryFormatter = new BinaryFormatter(m_SurrogateSelector, streamingContext);
	    }
	    
	    public void AddSerializableObject(SerializableObject serializableObject)
	    {
		    m_SerializableObjects.Add(serializableObject);
	    }

	    public void RemoveSerializableObject(SerializableObject serializableObject)
	    {
		    m_SerializableObjects.Remove(serializableObject);
	    }

	    public SerializableObject GetSerializableObject(string id)
	    {
		    return m_SerializableObjects.FirstOrDefault(n => n.ID == id);
	    }

	    [Button]
	    public void Save()
	    {
		    switch (m_SaveMode)
		    {
			    case SaveMode.Scene:
			    {
				    var data = m_SerializableObjects.ToList();
				    var path = Path.GetDirectoryName(m_SavePath) ?? Application.persistentDataPath;

				    Directory.CreateDirectory(path);
				    
				    using (var fs = File.Open(m_SavePath, FileMode.Create))
                    {
                        m_BinaryFormatter.Serialize(fs, data);
                        
                        // sync for web
                        if (Application.platform == RuntimePlatform.WebGLPlayer)
                            Extentions.SyncFiles();
                    }

			    }	break;
			    case SaveMode.Custom:
			    {
				    var data = m_CustomSave;
				    var path = Path.GetDirectoryName(m_SavePath) ?? Application.persistentDataPath;

				    Directory.CreateDirectory(path);

				    foreach (var n in data)
				    {
                        // save
					    using (var fs = File.Open(path + "/" +  n.gameObject.name + "_" + n.ID, FileMode.Create))
						    m_BinaryFormatter.Serialize(fs, data);

                        // sync for web
                        if (Application.platform == RuntimePlatform.WebGLPlayer)
                            Extentions.SyncFiles();
					    
				    }
			    }	break;
		    }
	    }
	    
	    [Button]
	    public void Load()
	    {
		    if (File.Exists(m_SavePath) == false)
			    return;
		    
		    switch (m_LoadMode)
		    {
			    case LoadMode.Default:
			    {
				    using (var fs = File.Open(m_SavePath, FileMode.Open))
				    {
					    var data = m_BinaryFormatter.Deserialize(fs) as List<SerializableObject>;
				    }
			    }	break;
			    case LoadMode.DestroyUnmodified:
			    {
				    // copy scene object, if loaded data doesn't contain some of them then destroy
				    var sceneObjectList = new HashSet<SerializableObject>(m_SerializableObjects);

				    using (var fs = File.Open(m_SavePath, FileMode.Open))
				    {
					    var data = m_BinaryFormatter.Deserialize(fs) as List<SerializableObject>;

					    foreach (var loadedObject in data)
						    sceneObjectList.Remove(loadedObject);

					    foreach (var unmodifiedObject in sceneObjectList)
						    Destroy(unmodifiedObject.gameObject);
				    }
			    }	break;
		    }
	    }

        //////////////////////////////////////////////////////////////////////////
	    public static string AssetPathToResourcePath(string resourcePath)
	    {
		    var indexOf = resourcePath.IndexOf(c_ResourcesPrefix, StringComparison.Ordinal);
		    return indexOf >= 0 ? 
			    Path.ChangeExtension(resourcePath.Substring(indexOf + c_ResourcesPrefix.Length), null) :
			    "";
	    }

        public static string ObjectToString(object toSerialize)
        {
            if (toSerialize == null)
                return string.Empty;

            var xmlSerializer = _GetXmlSerializer(toSerialize.GetType());

            using (var textWriter = new StringWriter())
            {
                xmlSerializer.Serialize(textWriter, toSerialize);
                return textWriter.ToString();
            }
        }

        public static T ObjectFromString<T>(string data)
        {
            if (string.IsNullOrEmpty(data))
                return default;

            try
            {
                return (T)ObjectFromString(typeof(T), data);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return default;
            }
        }

        public static object ObjectFromString(Type type, string data)
        {
            if (string.IsNullOrEmpty(data))
                return default;

            try
            {
                var xmlSerializer = new XmlSerializer(type);

                using (TextReader textReader = new StringReader(data))
                    return xmlSerializer.Deserialize(textReader);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return default;
            }
        }
        
        //////////////////////////////////////////////////////////////////////////
        private static XmlSerializer _GetXmlSerializer(Type type)
        {
            if (c_XmlSerializers.TryGetValue(type, out var serializer))
                return serializer;

            var xmlSerializer = new XmlSerializer(type);
            c_XmlSerializers.Add(type, xmlSerializer);

            return xmlSerializer;
        }
    }
}