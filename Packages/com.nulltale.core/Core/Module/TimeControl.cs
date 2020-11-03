using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Malee;

namespace Core.Module
{
    [CreateAssetMenu(fileName = nameof(TimeControl), menuName = Core.c_CoreModuleMenu + nameof(TimeControl))]
    public class TimeControl : Core.Module
    {
        public static TimeControl              Instance;

        [SerializeField]
        private bool					m_ControlPhysics;

        [SerializeField]
        private bool					m_CreateTimeController;
        [SerializeField, DrawIf(nameof(m_CreateTimeController), true)]
        private bool					m_EnableKeyControls;

        [SerializeField, Range(0.0f, 10.0f)]
        private float                   m_InitialGameSpeed = 1.0f;
        private float	                m_InitialFixedDeltaTime;
        private float                   m_GameSpeed = 1.0f;
        
        [SerializeField]
        [Reorderable(elementNameProperty = "m_Name")]
        public ReorderableArrayT<TimeScaleGroup> m_GroupList;
        
        public float                    TimeScale
        {
            get => Time.timeScale;
            set
            {
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (Time.timeScale != value) 
                    Time.timeScale = value;
                    
                // apply physics scale if game speed not zero
                if (Instance.m_ControlPhysics && value > 0.0f)
                    Instance.FixedDeltaTime = Instance.m_InitialFixedDeltaTime * value;
            }
        }

        public float                    FixedDeltaTime
        {
            get => Time.fixedDeltaTime;
            set
            {
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (Time.fixedDeltaTime != value) 
                    Time.fixedDeltaTime = value;
            }
        }

        //////////////////////////////////////////////////////////////////////////
        public class TimeLayerValue
        {
            private readonly TimeScaleGroup     m_Group;
            private int                         m_Layer;

            public bool                         IsOpen => m_Layer >= 0;
            public float                        Value
            {
                get => m_Group.m_Layers.TryGetValue(m_Layer, out var value) ? value : 0.0f;
                set => m_Layer = SetLayer(value, m_Group, m_Layer);
            }

            //////////////////////////////////////////////////////////////////////////
            public TimeLayerValue(TimeScaleGroup.LayerData layerData)
            {
                // set initial values
                m_Group = layerData.Group;
                m_Layer = layerData.Layer;
            }

            public void Close()
            {
                // close layer of the group & discard layer index to -1 (result of close function)
                m_Layer = SetLayer(0, m_Group, m_Layer);
            }
        }

        [Serializable]
        public class TimeScaleGroup
        {
            public bool                         m_Active;
            [Tooltip("Editor name")]
            public string                       m_Name;
            public int                          m_Index;
            public Method                       m_Method = Method.Multiplication;
            public Behaviour                    m_Behaviour = Behaviour.Average;
            public Dictionary<int, float>       m_Layers = new Dictionary<int, float>();

            public int                          TopLayerIndex => m_Layers.Max(n => n.Key);
            public bool                         Active => m_Active && m_Layers.IsEmpty() == false;
            public float                        Value
            {
                get
                {
                    switch (m_Behaviour)
                    {
                        case Behaviour.Max:
                            return m_Layers.Max(n => n.Value);
                        case Behaviour.Min:
                            return m_Layers.Min(n => n.Value);
                        case Behaviour.Multiplication:
                            return m_Layers.Aggregate(1f, (v, n) => v * n.Value);
                        case Behaviour.Average:
                            return m_Layers.Sum(n => n.Value) / m_Layers.Count;
                        case Behaviour.TopMost:
                            return m_Layers.MaxBy(n => n.Key).Value;
                        default:
                            throw new ArgumentOutOfRangeException();
                    };
                }
            }


            //////////////////////////////////////////////////////////////////////////
            // application of group value
            public enum Method
            {
                None                = 0,
                Multiplication,
                SingleMultiplyer,
            }
            
            // group value calculation method
            public enum Behaviour
            {
                Min,
                Max,
                Multiplication,
                Average,
                TopMost
            }
            
            public struct LayerData
            {
                public TimeScaleGroup   Group;
                public int              Layer;
                public float            Value;
            }

            //////////////////////////////////////////////////////////////////////////
            public LayerData GetLayerData(int layer)
            {
                // create new layer
                if (layer < 0)
                    layer = m_Layers.IsEmpty() ? 0 : m_Layers.Max(n => n.Key) + 1;
                
                // return existing or created value of layer
                return new LayerData() {Group = this, Layer = layer, Value = m_Layers[layer]};
            }

            public int SetLayer(int layer, float value)
            {
                // layer value allways positive
                if (value >= 0.0f)
                {   
                    // set layer
                    // apply or add layer value
                    if (layer < 0)
                        layer = m_Layers.IsEmpty() ? 0 : m_Layers.Max(n => n.Key) + 1;

                    m_Layers[layer] = value;

                }
                else
                {
                    // close layer
                    m_Layers.Remove(layer);
                    layer = -1;
                }

                return layer;
            }
        }

        //////////////////////////////////////////////////////////////////////////
        public override void Init()
        {
            Instance = this;
            m_InitialFixedDeltaTime = Time.fixedDeltaTime;

            // apply time scale
            SetGameSpeed(m_InitialGameSpeed);

            // create controller
            if (m_CreateTimeController)
            {
                var timeController = Core.Instance.gameObject.AddComponent<TimeController>();
                timeController.Init(m_InitialGameSpeed, m_EnableKeyControls);
            }
        }
        
        //////////////////////////////////////////////////////////////////////////
        public static void SetGameSpeed(float gameSpeed)
        {
            Instance.m_GameSpeed = gameSpeed;
            
            Instance._UpdateTimeScale();
        }
        
        public static int SetLayer(float value, Enum group, int layer = -1)
        {
            return SetLayer(value, Convert.ToInt32(group), layer);
        }

        public static int SetLayer(float value, int group, int layer = -1)
        {
            layer = SetLayer(value, Instance.m_GroupList[group], layer);

            return layer;
        }

        public static int SetLayer(float value, TimeScaleGroup group, int layer = -1)
        {
            layer = group.SetLayer(layer, value);

            Instance._UpdateTimeScale();

            return layer;
        }

        public static void CloseLayer(Enum group, int layer)
        {
            SetLayer(0, group, layer);
        }

        public static void CloseLayer(int group, int layer)
        {
            SetLayer(0, group, layer);
        }
        
        public static TimeLayerValue GetLayer(Enum group)
        {
            return GetLayer(Convert.ToInt32(group));
        }

        public static TimeLayerValue GetLayer(int group)
        {
            // always create new layer
            return new TimeLayerValue(Instance.m_GroupList[group].GetLayerData(-1));
        }

        //////////////////////////////////////////////////////////////////////////
        private void _UpdateTimeScale()
        {
            // calculate game speed
            var gameSpeed = m_GroupList.Aggregate(m_GameSpeed, (speed, group) => 
                {
                    // only for active groups
                    if (group.Active == false)
                        return speed;

                    // apply group effect
                    switch (group.m_Method)
                    {
                        // ignore group
                        case TimeScaleGroup.Method.None:
                            return speed;
                        // multiply
                        case TimeScaleGroup.Method.Multiplication:
                            return speed * group.Value;
                        // ignore multiplication history
                        case TimeScaleGroup.Method.SingleMultiplyer:
                            return m_GameSpeed * group.Value;

                        // unknow behaviour
                        default:
                            throw new Exception();
                    };
                });
                
            TimeScale = gameSpeed;
        }
    }
}