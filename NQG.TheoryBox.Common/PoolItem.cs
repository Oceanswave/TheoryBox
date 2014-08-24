namespace NQG.TheoryBox
{
    using System;

    /// <summary>
    /// Represents an item in the <see cref="ResourcePool{T}"/>
    /// </summary>
    /// <typeparam name="T">The type of the resource to be hold</typeparam>
    public sealed class PoolItem<T> : IDisposable where T : class
    {
        internal PoolItem(ResourcePool<T> pool, T resource)
        {
            m_pool = pool;
            m_resource = resource;
        }

        private T m_resource;
        private readonly ResourcePool<T> m_pool;

        public static implicit operator T(PoolItem<T> item)
        {
            return item.Resource;
        }

        /// <summary>
        /// Gets the resource hold by this resource pool item
        /// </summary>
        public T Resource { get { return m_resource; } }

        /// <summary>
        /// Disposes this instance of an resource pool item and sends the resource back into pool
        /// </summary>
        public void Dispose()
        {
            m_pool.SendBackToPool(m_resource);
            m_resource = null;
        }
    }
}
