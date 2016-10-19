using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrashBash.MonoGame.Objects
{
    public class Pool<T>
    {
        private ConcurrentBag<T> _objects;
        private Func<T> _objectGenerator;

        public Pool(Func<T> objectGenerator)
        {
            if (objectGenerator == null)
                throw new ArgumentNullException("objectGenerator");
            _objects = new ConcurrentBag<T>();
            _objectGenerator = objectGenerator;
        }

        public T Fetch()
        {
            T item;
            if (_objects.TryTake(out item)) return item;
            return _objectGenerator();
        }

        public void Insert(T item)
        {
            _objects.Add(item);
        }
    }
}
