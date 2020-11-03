using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    [Serializable]
    public class MatrixWrapper<T> : IDictionary<Vector2Int, T>, IEnumerable<T>
        where T : class, new()
    {
        [SerializeField]
        private List<T>	                m_Matrix;
        public int						Width { get; private set; }
        public int						Height { get; private set; }
        public int						Count => m_Matrix.Count;
        public bool						IsReadOnly => false;
        public ICollection<Vector2Int>	Keys => new IndexCollection() { m_Master = this };
        public ICollection<T>			Values => m_Matrix;

        public T this[Vector3Int key]
        {
            get
            {
                return this[key.x, key.y];
            }
            set
            {
                this[key.x, key.y] = value;
            }
        }

        public T this[Vector2Int key]
        {
            get
            {
                return this[key.x, key.y];
            }
            set
            {
                this[key.x, key.y] = value;
            }
        }

        public T this[int x, int y]
        {
            get
            {
                return m_Matrix[x * Height + y];
            }
            set
            {
                m_Matrix[x * Height + y] = value;
            }
        }

        //////////////////////////////////////////////////////////////////////////
        public class MatrixEnumerator : IEnumerator<KeyValuePair<Vector2Int, T>>
        {
            public MatrixWrapper<T>		m_Master;
            public int					m_Position = -1;

            //////////////////////////////////////////////////////////////////////////
            public bool MoveNext()
            {
                m_Position ++;
                return m_Position < m_Master.Count;
			
            }

            public void Reset()
            {
                m_Position = -1;
            }

            public KeyValuePair<Vector2Int, T> Current => new KeyValuePair<Vector2Int, T>(
                new Vector2Int(Mathf.FloorToInt(m_Position / (float)m_Master.Height), 
                    m_Position % m_Master.Height), m_Master.m_Matrix[m_Position]);

            object IEnumerator.Current => Current;

            public void Dispose()
            {
            }
        }

        public class IndexCollection : ICollection<Vector2Int>
        {
            public MatrixWrapper<T>		m_Master;

            public int					Count => m_Master.Count;
            public bool					IsReadOnly => true;

            //////////////////////////////////////////////////////////////////////////
            public class IndexEnumerator : IEnumerator<Vector2Int> 
            {
                public MatrixWrapper<T>		m_Master;
                public int					m_Position = -1;

                //////////////////////////////////////////////////////////////////////////
                public bool MoveNext()
                {
                    m_Position ++;
                    return m_Position < m_Master.Count;
			
                }

                public void Reset()
                {
                    m_Position = -1;
                }

                public Vector2Int Current => new Vector2Int(Mathf.FloorToInt(m_Position / (float)m_Master.Height), m_Position % m_Master.Height);

                object IEnumerator.Current => Current;

                public void Dispose()
                {
                }
            }

            //////////////////////////////////////////////////////////////////////////
            public IEnumerator<Vector2Int> GetEnumerator()
            {
                return new IndexEnumerator() {m_Master = m_Master};
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public void Add(Vector2Int item)
            {
                throw new NotImplementedException();
            }

            public void Clear()
            {
                throw new NotImplementedException();
            }

            public bool Contains(Vector2Int item)
            {
                return m_Master.InBounds(item);
            }

            public void CopyTo(Vector2Int[] array, int arrayIndex)
            {
                throw new NotImplementedException();
            }

            public bool Remove(Vector2Int item)
            {
                throw new NotImplementedException();
            }

        }

        //////////////////////////////////////////////////////////////////////////
        public void Allocate(int width, int height)
        {
            Allocate(width, height, (x, y) => default);
        }

        public void Allocate(int width, int height, Func<int, int, T> allocateFunction)
        {
            Width = width;
            Height = height;

            m_Matrix = new List<T>(Width * Height);

            // set allocate function
            if (allocateFunction == null)
                allocateFunction = (x, y) => new T();

            // allocate matrix
            for (var x = 0; x < Width; x++)
            for (var y = 0; y < Height; y++)
            {
                m_Matrix.Add(allocateFunction(x, y));
            }
        }

        public void Proceeded(Action<T, int, int> action)
        {
            for (var x = 0; x < Width; x++)
            for (var y = 0; y < Height; y++)
                action(this[x, y], x, y);
        }
        public void Proceeded(Action<T, Vector2Int> action)
        {
            for (var x = 0; x < Width; x++)
            for (var y = 0; y < Height; y++)
                action(this[x, y], new Vector2Int(x, y));
        }

        public bool TryGetValue(int x, int y, out T value)
        {
            if (InBounds(x, y))
            {
                value = this[x, y];
                return true;
            }

            value = null;
            return false;
        }

        public bool TryGetValue(Vector2Int key, out T value)
        {
            return TryGetValue(key.x, key.y, out value);
        }

        public List<T> GetValueList(params Vector2Int[] key)
        {
            var result = new List<T>(key.Length);
            foreach (var index in key)
            {
                if (TryGetValue(index.x, index.y, out var value))
                    result.Add(value);
            }

            return result;
        }

        public bool TryGetValue(Vector3Int key, out T value)
        {
            return TryGetValue(key.x, key.y, out value);
        }

        public bool InBounds(int x, int y)
        {
            return x >= 0 && x < Width && y >= 0 && y < Height;
        }

        public bool InBounds(Vector2Int pos)
        {
            return InBounds(pos.x, pos.y);
        }

	
        public TClone Clone<TClone>(Func<T, T> cloneFunction) where TClone : MatrixWrapper<T>, new()
        {
            var result = new TClone();

            result.Allocate(Width, Height, (x, y) => cloneFunction(this[x, y]));

            return result;
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return m_Matrix.GetEnumerator();
        }

        public IEnumerator<KeyValuePair<Vector2Int, T>> GetEnumerator()
        {
            return new MatrixEnumerator(){m_Master = this};
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(KeyValuePair<Vector2Int, T> item)
        {
            this[item.Key] = item.Value;
        }

        public void Clear()
        {
            Width = 0;
            Height = 0;
            m_Matrix.Clear();
        }

        public bool Contains(KeyValuePair<Vector2Int, T> item)
        {
            if (TryGetValue(item.Key, out var value))
                return value.Equals(item.Value);

            return false;
        }

        public void CopyTo(KeyValuePair<Vector2Int, T>[] array, int arrayIndex)
        {
            for (var x = 0; x < Width; x++)
            for (var y = 0; y < Height; y++)
            {
                array[arrayIndex + x * Height + y] = new KeyValuePair<Vector2Int, T>(new Vector2Int(x, y), this[x, y]);
            }
        }

        public bool Remove(KeyValuePair<Vector2Int, T> item)
        {
            if (InBounds(item.Key) && this[item.Key] == item.Value)
            {
                this[item.Key] = null;
                return true;
            }

            return false;
        }

        public void Add(Vector2Int key, T value)
        {
            this[key] = value;
        }

        public bool ContainsKey(Vector2Int key)
        {
            return InBounds(key);
        }

        public bool Remove(Vector2Int key)
        {
            if (InBounds(key))
            {
                this[key] = null;
                return true;
            }

            return false;
        }
    }
}