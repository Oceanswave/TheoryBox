namespace NQG.TheoryBox
{
    using System;
    using System.Threading;

    public static class ThreadSafeRandom
    {
        [ThreadStatic]
        private static Random s_local;

        public static Random ThisThreadsRandom
        {
            get { return s_local ?? (s_local = new Random(unchecked(Environment.TickCount * 31 + Thread.CurrentThread.ManagedThreadId))); }
        }
    }
}
