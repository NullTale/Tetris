using UnityEngine;
using System.Runtime.Serialization;

namespace Core
{
    public static class UnityCommon_Serializator
    {
        public class SerializationSurrogate_Vector3 : ISerializationSurrogate
        {
            // Method called to serialize a Vector3 object
            public void GetObjectData(System.Object obj, SerializationInfo info, StreamingContext context)
            {
                Vector3 v = (Vector3)obj;
                info.AddValue("x", v.x);
                info.AddValue("y", v.y);
                info.AddValue("z", v.z);
            }
 
            // Method called to deserialize a Vector3 object
            public System.Object SetObjectData(System.Object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
            {
                Vector3 v = (Vector3)obj;
                v.x = (float)info.GetValue("x", typeof(float));
                v.y = (float)info.GetValue("y", typeof(float));
                v.z = (float)info.GetValue("z", typeof(float));

                return v;
            }
        }
        public class SerializationSurrogate_Vector3Int : ISerializationSurrogate
        {
            public void GetObjectData(System.Object obj, SerializationInfo info, StreamingContext context)
            {
                var v = (Vector3Int)obj;
                info.AddValue("x", v.x);
                info.AddValue("y", v.y);
                info.AddValue("z", v.z);
            }
 
            public System.Object SetObjectData(System.Object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
            {
                var v = (Vector3Int)obj;
                v.x = info.GetInt32("x");
                v.y = info.GetInt32("y");
                v.z = info.GetInt32("z");

                return v;
            }
        }
        public class SerializationSurrogate_Vector2Int : ISerializationSurrogate
        {
            public void GetObjectData(System.Object obj, SerializationInfo info, StreamingContext context)
            {
                var v = (Vector2Int)obj;
                info.AddValue("x", v.x);
                info.AddValue("y", v.y);
            }
 
            public System.Object SetObjectData(System.Object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
            {
                var v = (Vector2Int)obj;
                v.x = info.GetInt32("x");
                v.y = info.GetInt32("y");

                return v;
            }
        }
        public class SerializationSurrogate_Vector2 : ISerializationSurrogate
        {
            public void GetObjectData(System.Object obj, SerializationInfo info, StreamingContext context)
            {
                var v = (Vector2)obj;
                info.AddValue("x", v.x);
                info.AddValue("y", v.y);
            }
 
            public System.Object SetObjectData(System.Object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
            {
                var v = (Vector2)obj;
                v.x = info.GetInt32("x");
                v.y = info.GetInt32("y");

                return v;
            }
        }
        public class SerializationSurrogate_Quaternion : ISerializationSurrogate
        {
            public void GetObjectData(System.Object obj, SerializationInfo info, StreamingContext context)
            {
                var q = (Quaternion)obj;
                info.AddValue("x", q.x);
                info.AddValue("y", q.y);
                info.AddValue("z", q.z);
                info.AddValue("w", q.w);
            }
 
            public System.Object SetObjectData(System.Object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
            {
                var q = (Quaternion)obj;
                q.x = (float)info.GetValue("x", typeof(float));
                q.y = (float)info.GetValue("y", typeof(float));
                q.z = (float)info.GetValue("z", typeof(float));
                q.w = (float)info.GetValue("w", typeof(float));

                return q;
            }
        }
        public class SerializationSurrogate_Color : ISerializationSurrogate
        {
            public void GetObjectData(System.Object obj, SerializationInfo info, StreamingContext context)
            {
                var color = (Color)obj;
                info.AddValue("r", color.r);
                info.AddValue("g", color.g);
                info.AddValue("b", color.b);
                info.AddValue("a", color.a);
            }
 
            public System.Object SetObjectData(System.Object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
            {
                var color = (Color)obj;
                color.r = (float)info.GetValue("r", typeof(float));
                color.g = (float)info.GetValue("g", typeof(float));
                color.b = (float)info.GetValue("b", typeof(float));
                color.a = (float)info.GetValue("a", typeof(float));

                return color;
            }
        }
	
        public class Empty_Serializator<T>
        {
            private class Serializator_Empty : ISerializationSurrogate
            {
                public void GetObjectData(System.Object obj, SerializationInfo info, StreamingContext context)	{}
 
                public System.Object SetObjectData(System.Object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)	{return obj;}
            }

            public static void Reg(SurrogateSelector selector)
            {
                selector.AddSurrogate(typeof(T), new StreamingContext(StreamingContextStates.All), new Serializator_Empty());
            }
        }

        public static void Reg(SurrogateSelector selector)
        {
            selector.AddSurrogate(typeof(Vector2), new StreamingContext(StreamingContextStates.All), new SerializationSurrogate_Vector2());
            selector.AddSurrogate(typeof(Vector3), new StreamingContext(StreamingContextStates.All), new SerializationSurrogate_Vector3());
            selector.AddSurrogate(typeof(Vector2Int), new StreamingContext(StreamingContextStates.All), new SerializationSurrogate_Vector2Int());
            selector.AddSurrogate(typeof(Vector3Int), new StreamingContext(StreamingContextStates.All), new SerializationSurrogate_Vector3Int());
            selector.AddSurrogate(typeof(Quaternion), new StreamingContext(StreamingContextStates.All), new SerializationSurrogate_Quaternion());
            selector.AddSurrogate(typeof(Color), new StreamingContext(StreamingContextStates.All), new SerializationSurrogate_Color());
        }
    }
}