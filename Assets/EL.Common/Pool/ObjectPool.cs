using System;
using System.Collections.Generic;

namespace EL.Common.Pool
{
    public class ObjectPool : IObjectPool
    {
        private readonly Dictionary<Type, object> _pools = new Dictionary<Type, object>();

        public void Register<T>(IPool<T> pool)
        {
            var type = typeof(T);
            if (_pools.ContainsKey(type))
                throw new ArgumentException($"Pool of Type {type} already created");
            _pools[type] = pool;
        }

        public bool CanTakeFromPool<T>()
        {
            return GetPoll<T>().CanTake();
        }

        public T Take<T>()
        {
            return GetPoll<T>().Take();
        }

        public void Release<T>(T item)
        {
            GetPoll<T>().Release(item);
        }

        public IPool<T> GetPoll<T>()
        {
            var type = typeof(T);
            if (!_pools.ContainsKey(type))
                throw new AggregateException($"Pool of type {type} does not exists");
            return (IPool<T>) _pools[type];
        }
    }
}