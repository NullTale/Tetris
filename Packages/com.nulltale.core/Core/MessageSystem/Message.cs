
using System.Security.Cryptography;

namespace Core.EventSystem
{
    public interface IMessageBase {}

    public interface IMessage<T> : IMessageBase
    {
        T Key           { get; set; }
        string Channel  { get; set; }
    }

    public interface IMessageData : IMessageBase
    {
        object Data { get; set; }
    }

    public class Message<T> : IMessage<T>
    {
        public T Key            { get; set; }
        public string Channel   { get; set; }

        //////////////////////////////////////////////////////////////////////////
        public Message(T key, string channel = null)
        {
            Key = key;
            Channel = channel;
        }
    }

    public class MessageData<T> : Message<T>, IMessageData
    {
        public object Data { get; set; }

        //////////////////////////////////////////////////////////////////////////
        public MessageData(T key, object data) 
            : base(key)
        {
            Data = data;
        }
        
        public MessageData(T key, string channel, object data) 
            : base(key, channel)
        {
            Data = data;
        }
    }

    public static class EventExtentions
    {
        public static T GetData<T>(this IMessageBase e)
        {
            // try get data
            if (e is IMessageData eventData && eventData.Data is T dataCast)
                return dataCast;

            return default;
        }
        
        public static bool TryGetData<T>(this IMessageBase e, out T data)
        {
            // try get data
            if (e is IMessageData eventData && eventData.Data is T dataCast)
            {
                data = dataCast;
                return true;
            }

            data = default;
            return false;
        }

        public static IMessage<T> SetData<D, T>(this IMessage<T> e, D data)
        {
            // set data
            if (e is IMessageData eventData)
                eventData.Data = data;
            
            return e;
        }
        
        public static IMessage<T> Send<T>(this IMessage<T> e, string channel = MessageSystem.c_DefaultChannel)
        {
            MessageSystem.Send(e, channel);
            return e;
        }
        
        public static IMessage<T> Send<T>(this IMessage<T> e)
        {
            MessageSystem.Send(e);
            return e;
        }
        
        public static void Use<T>(this IMessage<T> e)
        {
            MessageSystem.Instance.SkipEvent = true;
        }
        
        #region Deconstructors
        public static (T1, T2) GetData<T1, T2>(this IMessageBase e)
        {
            // try get data
            var dataArray = e.GetData<object[]>();

            return ((T1)dataArray[0], (T2)dataArray[1]);
        }

        public static (T1, T2, T3) GetData<T1, T2, T3>(this IMessageBase e)
        {
            // try get data
            var dataArray = e.GetData<object[]>();

            return ((T1)dataArray[0], (T2)dataArray[1], (T3)dataArray[2]);
        }

        public static (T1, T2, T3, T4) GetData<T1, T2, T3, T4>(this IMessageBase e)
        {
            // try get data
            var dataArray = e.GetData<object[]>();

            return ((T1)dataArray[0], (T2)dataArray[1], (T3)dataArray[2], (T4)dataArray[3]);
        }

        public static (T1, T2, T3, T4, T5) GetData<T1, T2, T3, T4, T5>(this IMessageBase e)
        {
            // try get data
            var dataArray = e.GetData<object[]>();

            return ((T1)dataArray[0], (T2)dataArray[1], (T3)dataArray[2], (T4)dataArray[3], (T5)dataArray[4]);
        }

        public static (T1, T2, T3, T4, T5, T6) GetData<T1, T2, T3, T4, T5, T6>(this IMessageBase e)
        {
            // try get data
            var dataArray = e.GetData<object[]>();

            return ((T1)dataArray[0], (T2)dataArray[1], (T3)dataArray[2], (T4)dataArray[3],
                (T5)dataArray[4], (T6)dataArray[5]);
        }

        public static (T1, T2, T3, T4, T5, T6, T7) GetData<T1, T2, T3, T4, T5, T6, T7>(this IMessageBase e)
        {
            // try get data
            var dataArray = e.GetData<object[]>();

            return ((T1)dataArray[0], (T2)dataArray[1], (T3)dataArray[2], (T4)dataArray[3],
                (T5)dataArray[4], (T6)dataArray[5], (T7)dataArray[6]);
        }

        public static (T1, T2, T3, T4, T5, T6, T7, T8) GetData<T1, T2, T3, T4, T5, T6, T7, T8>(this IMessageBase e)
        {
            // try get data
            var dataArray = e.GetData<object[]>();

            return ((T1)dataArray[0], (T2)dataArray[1], (T3)dataArray[2], (T4)dataArray[3],
                (T5)dataArray[4], (T6)dataArray[5], (T7)dataArray[6], (T8)dataArray[7]);
        }
        #endregion
    }
}