namespace NQG.TheoryBox
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading;

    /// <summary>
    /// Generic pool class
    /// </summary>
    /// <typeparam name="T">The type of items to be stored in the pool</typeparam>
    public class ResourcePool<T> : IDisposable where T : class
    {
        /// <summary>
        /// Creates a new pool
        /// </summary>
        /// <param name="factory">The factory method to create new items to be stored in the pool</param>
        public ResourcePool(Func<ResourcePool<T>, T> factory)
        {
            if (factory == null)
                throw new ArgumentNullException("factory");
            m_factoryMethod = factory;
        }

        private readonly Func<ResourcePool<T>, T> m_factoryMethod;
        private ConcurrentQueue<PoolItem<T>> m_freeItems = new ConcurrentQueue<PoolItem<T>>();
        private ConcurrentQueue<AutoResetEvent> m_waitLocks = new ConcurrentQueue<AutoResetEvent>();

        private ConcurrentDictionary<AutoResetEvent, PoolItem<T>> m_syncContext =
            new ConcurrentDictionary<AutoResetEvent, PoolItem<T>>();

        public Action<T> CleanupPoolItem { get; set; }

        /// <summary>
        /// Gets the current count of items in the pool
        /// </summary>
        public int Count { get; private set; }

        public void Dispose()
        {
            lock (this)
            {
                if (Count != m_freeItems.Count)
                    throw new InvalidOperationException(
                       "Cannot dispose the resource pool while one or more pooled items are in use");

                foreach (var poolItem in m_freeItems)
                {
                    var cleanMethod = CleanupPoolItem;
                    if (cleanMethod != null)
                        CleanupPoolItem(poolItem.Resource);
                }

                Count = 0;
                m_freeItems = null;
                m_waitLocks = null;
                m_syncContext = null;
            }
        }

        /// <summary>
        /// Gets a free resource from the pool. If no free items available this method tries to 
        /// create a new item. If no new item could be created this method waits until another thread
        /// frees one resource.
        /// </summary>
        /// <returns>A resource item</returns>
        public PoolItem<T> GetItem()
        {
            PoolItem<T> item;

            // try to get an item
            if (TryGetItem(out item))
                return item;

            AutoResetEvent waitLock = null;

            lock (this)
            {
                // try to get an entry in exclusive mode
                if (!TryGetItem(out item))
                {
                    // no item available, create a wait lock and enqueue it
                    waitLock = new AutoResetEvent(false);
                    m_waitLocks.Enqueue(waitLock);
                }
            }

            if (waitLock == null)
                return item;

            // wait until a new item is available
            waitLock.WaitOne();
            m_syncContext.TryRemove(waitLock, out item);
            waitLock.Dispose();

            return item;
        }

        private bool TryGetItem(out PoolItem<T> item)
        {
            // try to get an already pooled resource
            if (m_freeItems.TryDequeue(out item))
                return true;

            lock (this)
            {
                // try to create a new resource
                var resource = m_factoryMethod(this);
                if (resource == null && Count == 0)
                    throw new InvalidOperationException("Pool empty and no item created");

                if (resource != null)
                {
                    // a new resource was created and can be returned
                    Count++;
                    item = new PoolItem<T>(this, resource);
                }
                else
                {
                    // no items available to return at the moment
                    item = null;
                }

                return item != null;
            }
        }

        /// <summary>
        /// Called from <see cref="PoolItem{T}"/> to free previously taked resources
        /// </summary>
        /// <param name="resource">The resource to send back into the pool.</param>
        internal void SendBackToPool(T resource)
        {
            lock (this)
            {
                var item = new PoolItem<T>(this, resource);
                AutoResetEvent waitLock;

                if (m_waitLocks.TryDequeue(out waitLock))
                {
                    m_syncContext.TryAdd(waitLock, item);
                    waitLock.Set();
                }
                else
                {
                    m_freeItems.Enqueue(item);
                }
            }
        }
    }
}