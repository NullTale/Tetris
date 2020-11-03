using System;
using System.Collections.Generic;

namespace Core
{
    public class ListGroup<T, L> : ListGroup<T, L, ListGroup<T, L>.LGDictionary>
    {
        public class LGDictionary : Dictionary<L, ListGroup<T, L, LGDictionary>.Group> {}
    }

    public class ListGroup<T, L, Dic> where Dic : class, IDictionary<L, ListGroup<T, L, Dic>.Group>, new()
    {
        public List<T>                  m_Data;
        private Dic                     m_GroupDictionary = new Dic();

        public List<T>                  this[L lable] => m_GroupDictionary[lable].Data;

        //////////////////////////////////////////////////////////////////////////
        [Serializable]
        public class Group
        {
            public List<T>          Data;
            public Func<T, bool>    Filter;
            public Action<T>        Added;
            public Action<T>        Removed;
        }

        //////////////////////////////////////////////////////////////////////////
        public virtual void Init()
        {
            foreach (var item in m_Data)
                Set(item);
        }

        public void Set(T item)
        {
            // disallow null items
            if (item == null)
                return;

            // add or update item
            if (m_Data.Contains(item) == false)
            {
                // add new item
                m_Data.Add(item);

                // try add to all groups
                foreach (var group in m_GroupDictionary.Values)
                {
                    if (group.Filter(item))
                    {
                        group.Data.Add(item);
                        group.Added?.Invoke(item);
                    }
                }
            }
            else
            {
                // update item status in all groups
                foreach (var group in m_GroupDictionary.Values)
                {
                    if (group.Filter(item))
                    {   
                        // add or do nothing if already added
                        if (group.Data.Contains(item) == false)
                        {
                            group.Data.Add(item);
                            group.Added?.Invoke(item);
                        }
                    }
                    else
                    {
                        // remove or do nothing if not presented in the list
                        if (group.Data.Contains(item))
                        {
                            group.Data.Remove(item);
                            group.Removed?.Invoke(item);
                        }
                    }
                }
            }
        }

        public void Remove(T item)
        {
            // remove from groups if presented in main list
            if (m_Data.Remove(item) == false)
                return;

            // update item status in all groups
            foreach (var group in m_GroupDictionary.Values)
            {
                if (group.Data.Contains(item))
                {
                    group.Data.Remove(item);
                    group.Removed?.Invoke(item);
                }
            }

            return;
        }

        public void AddGroup(L lable, Func<T, bool> itemFilter, Action<T> itemAdded = null, Action<T> itemRemoved = null)
        {
            // add group
            var group = new Group() 
            {
                Data = new List<T>(),
                Filter = itemFilter,
                Added = itemAdded,
                Removed = itemRemoved
            };

            m_GroupDictionary.Add(lable, group);

            // add items to the group
            foreach (var item in m_Data)
            {
                if (group.Filter(item))
                {
                    group.Data.Add(item);
                    group.Added?.Invoke(item);
                }
            }
        }
    }
}