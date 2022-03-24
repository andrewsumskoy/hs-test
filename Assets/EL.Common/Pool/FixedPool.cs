using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace EL.Common.Pool
{
    public class FixedPool<T, TS> : IPool<T>
    {
        private readonly List<object> _items;
        private readonly string _name;
        private readonly IPoolItemObserver<T, TS> _observer;
        private readonly List<object> _taken;

        public FixedPool(string name, IPoolItemObserver<T, TS> observer)
        {
            _name = name;
            _observer = observer;
            _items = new List<object>();
            _taken = new List<object>();
        }

        public bool CanTake()
        {
            return _items.Count > 0;
        }

        public T Take()
        {
            if (_items.Count == 0)
                throw new ArgumentException($"Pool {_name}: has now more elements for take");
            var item = _items[0];
            _items.RemoveAt(0);
            _taken.Add(item);
            _observer?.AfterTake((T) item);
            return (T) item;
        }

        public void Release(T item)
        {
            var o = (object) item;
            if (!_taken.Contains(o))
                throw new ArgumentException($"Pool {_name}: you try release not taken element");
            _taken.Remove(o);
            _items.Add(o);
            _observer?.AfterReturn(item);
        }

        public void Add(TS item)
        {
            _items.Add(_observer != null ? _observer.Create(item) : (object) item);
        }

        public async UniTask AddRange(IEnumerable<TS> items, int batchCount = 50)
        {
            var i = 0;
            foreach (var item in items)
            {
                if (i % batchCount == 0)
                    await UniTask.Yield();
                Add(item);
                i++;
            }
        }
    }
}