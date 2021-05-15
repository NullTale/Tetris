using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using Core;
using Core.EventSystem;
using Core.Module;
using Malee;
using RotaryHeart.Lib.SerializableDictionary;
using UnityEditor;
using UnityEngine;
using Utilities;
using Action = System.Action;
using NaughtyAttributes;
using UnityEngine.Scripting;
using Object = System.Object;

[assembly:Preserve]

namespace Core
{
    [DisallowMultipleComponent, DefaultExecutionOrder(-1000)]
    public class Core : MonoBehaviour
    {
        public interface IModule
        {
           void Init();
        }

        public abstract class Module : ScriptableObject, IModule
        {
            public abstract void Init();
        }

        [Serializable]
        public class ModuleWrapper
        {
            [SerializeField]
            private bool                    m_Active;
            [SerializeField]
            [Expandable]
            private UnityEngine.Object      m_Module;

            private IModule                 m_Instance;

            public IModule                  Module => m_Active ? m_Instance : null;

            //////////////////////////////////////////////////////////////////////////
            public void Init()
            {
                if (m_Active == false)
                    return;

                if (m_Module == null)
                    return;

                switch (m_Module)
                {
                    case GameObject go:
                    {
                        // instantiate if instance not a scene object
                        m_Instance = go.gameObject.scene.name == null ? (Instantiate(go, Core.Instance.transform) as GameObject).GetComponent<IModule>() : go.GetComponent<IModule>();
                    } break;

                    case ScriptableObject so:
                        m_Instance = m_Module as IModule;
                        break;

                    default:
                        m_Instance = null;
                        break;
                }

                // init module
                m_Instance?.Init();
            }
        }

        [Serializable]
        public class ModuleList : ReorderableArray<ModuleWrapper> {}

        [Serializable]
        public enum MouseButton : int
        {
            None    = -1,

            Left    = 0,
            Right   = 1,
            Middle  = 2
        }

        [Serializable]
        public enum ConditionBool
        {
            True,
            False,
            Any,
            None,
        }

        [Serializable]
        public enum LogicOperation
        {
            And,
            Or,
            Equal,
            NotEqual,
        }

        [Serializable]
        public enum ComparisonOperation
        {
            Less,
            Greater,
            Equal,
            NotEqual,
            LessOrEqual,
            GreaterOrEqual,
            Any,
            None
        }

        [Serializable]
        public enum Tribool
        {
            False,
            True,
            Unknown,
        }
    
        [Serializable]
        public enum ProjectSpace
        {
            XY,
            XZ
        }

        //////////////////////////////////////////////////////////////////////////
        private const string		        c_DefaultDelimiterObjectName = "----------//----------";
        public const string                 c_CoreModuleMenu = "Core Module/";

        public const int					c_ManagerDefaultExecutionOrder = -10;

        public static readonly Vector2		c_Vector2LeftTop     = new Vector2(-0.707106769084930419921875f, 0.707106769084930419921875f);
        public static readonly Vector2		c_Vector2RightTop    = new Vector2(0.707106769084930419921875f, 0.707106769084930419921875f);
        public static readonly Vector2		c_Vector2LeftBottom  = new Vector2(-0.707106769084930419921875f, -0.707106769084930419921875f);
        public static readonly Vector2		c_Vector2RightBottom = new Vector2(0.707106769084930419921875f, -0.707106769084930419921875f);
										
        public static readonly Vector3		c_Vector3LeftTop     = new Vector3(-0.707106769084930419921875f, 0.0f, 0.707106769084930419921875f);
        public static readonly Vector3		c_Vector3RightTop    = new Vector3(0.707106769084930419921875f, 0.0f, 0.707106769084930419921875f);
        public static readonly Vector3		c_Vector3LeftBottom  = new Vector3(-0.707106769084930419921875f, 0.0f, -0.707106769084930419921875f);
        public static readonly Vector3		c_Vector3RightBottom = new Vector3(0.707106769084930419921875f, 0.0f, -0.707106769084930419921875f);
    
        public static Core			        m_Instance;
        public static Core			        Instance => m_Instance ? m_Instance : (m_Instance = FindObjectOfType<Core>());

        [SerializeField]
        private Camera				        m_Camera;
        public Camera				        Camera => m_Camera;

        public bool						    m_DoNotDestroyOnLoad = true;
        
        [SerializeField]
        private ThreadPriority              m_LoadingPriority;
	
        [SerializeField, Reorderable(elementNameProperty = "m_Module", surrogateType = typeof(Module), surrogateProperty = "m_Module")]
        private ModuleList                  m_Modules;

        public MousePosition                MouseWorldPosition  {get; private set;}
        public FPSCounter                   FPSCounter  {get; private set;}
        public Localization                 Localization  {get; private set;}

        //////////////////////////////////////////////////////////////////////////
        private void Awake()
        {
            Application.backgroundLoadingPriority = m_LoadingPriority;
            
            // set instance
            m_Instance = this;

            // get main camera if not set
            m_Camera = m_Camera ? m_Camera : Camera.main;

            // init modules
            foreach (var module in m_Modules)
                module.Init();

            // set most common modules
            MouseWorldPosition = GetModule<MousePosition>();
            FPSCounter = GetModule<FPSCounter>();
            Localization = GetModule<Localization>();


            // set do not destroy on load
            if (m_DoNotDestroyOnLoad)
                DontDestroyOnLoad(gameObject);
        }

        private void Update() 
        {
#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.Pause))
                Debug.Break();
#endif
        }

        public void Log(string text)
        {
            Debug.Log(text);
        }

        public T GetModule<T>() where T : Module
        {
            return m_Modules.FirstOrDefault(n => n.Module is T)?.Module as T;
        }

#if UNITY_EDITOR
        //////////////////////////////////////////////////////////////////////////
        [MenuItem("GameObject/Delimiter", false, 10)]
        static void CreateCustomGameObject(MenuCommand menuCommand)
        {
            var go = new GameObject(c_DefaultDelimiterObjectName);
            go.tag = "EditorOnly";
            go.isStatic = true;
            go.SetActive(false);

            // Ensure it gets reparented if this was a context click (otherwise does nothing)
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);

            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            Selection.activeObject = go;
        }

        [MenuItem("GameObject/Remove Missing Scripts Recursively", false, 0)]
        private static void FindAndRemoveMissingInSelected()
        {
            var deepSelection = EditorUtility.CollectDeepHierarchy(Selection.gameObjects);
            int compCount = 0;
            int goCount = 0;
            foreach (var o in deepSelection)
            {
                if (o is GameObject go)
                {
                    int count = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(go);
                    if (count > 0)
                    {
                        // Edit: use undo record object, since undo destroy wont work with missing
                        Undo.RegisterCompleteObjectUndo(go, "Remove missing scripts");
                        GameObjectUtility.RemoveMonoBehavioursWithMissingScript(go);
                        compCount += count;
                        goCount++;
                    }
                }
            }
            Debug.Log($"Found and removed {compCount} missing scripts from {goCount} GameObjects");
        }
	
        [MenuItem("Edit/Reserialize Assets", false, 10)]
        static void ReserializeAssets(MenuCommand menuCommand)
        {
            AssetDatabase.ForceReserializeAssets();
        }

#endif
        //////////////////////////////////////////////////////////////////////////
        public static float Fib(float baseFib, int iterations, float stepLimit = Single.MaxValue)
        {
            var a = 0.0f;
            var b = baseFib;
            var c = 0.0f;

            for (var n = 0; n < iterations; n++)
            {
                c = Mathf.Min(a, stepLimit) + b;
                a = b;
                b = c;
            }

            return c;
        }

        public static void Fib(float baseFib, int iterations, List<float> values, float stepLimit = Single.MaxValue)
        {
            var a = 0.0f;
            var b = baseFib;

            values.Add(a);
            values.Add(b);

            for (var n = 0; n < iterations; n++)
            {
                var c = Mathf.Min(a, stepLimit) + b;
                a = b;
                b = c;
                values.Add(c);
            }
        }
	
        public static IEnumerable<T> GetFlags<T>(T input) where T : Enum
        {
            foreach (T value in Enum.GetValues(typeof(T)))
                if (input.HasFlag(value))
                    yield return value;
        }

        public static IEnumerable<T> GetEnum<T>() where T : Enum
        {
            return Enum.GetValues(typeof(T)).OfType<T>();
        }

        public static void DrawEllipse(Vector3 pos, float radius, Color color, int segments = 20, float duration = 0)
        {
            DrawEllipse(pos, Vector3.forward, Vector3.up, radius, radius, color, segments, duration);
        }

        public static void DrawEllipse(Vector3 pos, Vector3 forward, Vector3 up, float radiusX, float radiusY, Color color, int segments, float duration = 0)
        {
            float angle = 0f;
            Quaternion rot = Quaternion.LookRotation(forward, up);
            Vector3 lastPoint = Vector3.zero;
            Vector3 thisPoint = Vector3.zero;
 
            for (int i = 0; i < segments + 1; i++)
            {
                thisPoint.x = Mathf.Sin(Mathf.Deg2Rad * angle) * radiusX;
                thisPoint.y = Mathf.Cos(Mathf.Deg2Rad * angle) * radiusY;
 
                if (i > 0)
                {
                    Debug.DrawLine(rot * lastPoint + pos, rot * thisPoint + pos, color, duration);
                }
 
                lastPoint = thisPoint;
                angle += 360f / segments;
            }
        }

        public static T DeepCopy<T>(T other)
        {
            using(var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, other);
                ms.Position = 0;
                return (T)formatter.Deserialize(ms);
            }
        }
        public static T ShallowCopy<T>(T other)
        {
            return (T)Reflection.MakeShallowCopy(other, false);
        }
    }
    
    public class GameObjectIWorldPositionWrapper : IWorldPosition
    {
	    private GameObject		m_GameObject;

	    //////////////////////////////////////////////////////////////////////////
	    public Vector3 GetWorldPosition()
	    {
		    return m_GameObject.transform.position;
	    }

	    //////////////////////////////////////////////////////////////////////////
	    public GameObjectIWorldPositionWrapper(GameObject m_GameObject)
	    {
		    this.m_GameObject = m_GameObject;
	    }
    }

    public class GameObjectIWorldPositionWrapperEx : IWorldPosition
    {
	    private GameObject				m_GameObject;
	    private Action<GameObject>		m_OnDestroyAction;

	    //////////////////////////////////////////////////////////////////////////
	    public void Release()
	    {
		    m_GameObject.GetComponent<OnDestroyCallback>().OnDestroyAction -= m_OnDestroyAction;
	    }

	    //////////////////////////////////////////////////////////////////////////
	    public Vector3 GetWorldPosition()
	    {
		    return m_GameObject == null ? Vector3.zero : m_GameObject.transform.position;
	    }

	    //////////////////////////////////////////////////////////////////////////
	    public GameObjectIWorldPositionWrapperEx(GameObject m_GameObject, Action objectDestroyed)
	    {
		    m_OnDestroyAction = (obj) => { m_GameObject = null; };
		    if(objectDestroyed != null)
			    m_OnDestroyAction += (obj) =>  { objectDestroyed.Invoke(); };

		    var destroyCallback = m_GameObject.GetComponent<OnDestroyCallback>();
		    if(destroyCallback == null)
			    destroyCallback = m_GameObject.AddComponent<OnDestroyCallback>();

		    destroyCallback.OnDestroyAction += m_OnDestroyAction;


		    this.m_GameObject = m_GameObject;
	    }
    }

    public class WorldPositionVectorConst : IWorldPosition
    {
	    public static readonly WorldPositionVectorConst	c_Zero = new WorldPositionVectorConst(Vector3.zero);

	    private Vector3					m_Position;
	    
	    //////////////////////////////////////////////////////////////////////////
	    public Vector3 GetWorldPosition()
	    {
		    return m_Position;
	    }

	    //////////////////////////////////////////////////////////////////////////
	    public WorldPositionVectorConst(Vector3 m_Position)
	    {
		    this.m_Position = m_Position;
	    }
    }

    public class WorldPositionVector : IWorldPosition
    {
	    public Vector3				m_Position;

	    //////////////////////////////////////////////////////////////////////////
	    public Vector3 GetWorldPosition()
	    {
		    return m_Position;
	    }
	    
	    public WorldPositionVector()
	    {
	    }

	    public WorldPositionVector(Vector3 m_Position)
	    {
		    this.m_Position = m_Position;
	    }
    }

    public static class Actions
    {
	    public static void Empty() { }
	    public static void Empty<T>(T value) { }
	    public static void Empty<T1, T2>(T1 value1, T2 value2) { }
    }

    public static class Functions
    {
	    public static T Identity<T>(T value) { return value; }

	    public static T0 Default<T0>() { return default(T0); }
	    public static T0 Default<T1, T0>(T1 value1) { return default(T0); }

	    public static bool IsNull<T>(T entity) where T : class { return entity == null; }
	    public static bool IsNonNull<T>(T entity) where T : class { return entity != null; }

	    public static bool True<T>(T entity) { return true; }
	    public static bool False<T>(T entity) { return false; }
    }

    [Serializable]
    public class ReorderableArrayT<T> : ReorderableArray<T> {}
}
