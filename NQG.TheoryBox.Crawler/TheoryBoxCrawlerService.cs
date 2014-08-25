namespace NQG.TheoryBox.Crawler
{
    using System;
    using System.Threading;
    using Topshelf;

    public class TheoryBoxCrawlerService : ServiceControl, IDisposable
    {
        private readonly TheoryBoxCrawler m_crawler = new TheoryBoxCrawler();
        private Thread m_workerThread;
        private bool m_isDisposed;

        public TheoryBoxCrawlerService()
        {
            m_workerThread = new Thread(() =>
            {
                AsyncPump.Run(() => m_crawler.Initialize());
                AsyncPump.Run(() => m_crawler.Crawl());
            });
        }

        public bool Start(HostControl control)
        {
            m_workerThread.Start();
            return true;
        }

        public bool Stop(HostControl control)
        {
            if (m_workerThread.ThreadState == ThreadState.Running)
            {
                m_workerThread.Abort();
                m_workerThread = null;
            }

            return true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Dispose(bool disposing) executes in two distinct scenarios. 
        // If disposing equals true, the method has been called directly 
        // or indirectly by a user's code. Managed and unmanaged resources 
        // can be disposed. 
        // If disposing equals false, the method has been called by the 
        // runtime from inside the finalizer and you should not reference 
        // other objects. Only unmanaged resources can be disposed. 
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called. 
            if (m_isDisposed)
                return;

            // If disposing equals true, dispose all managed 
            // and unmanaged resources. 
            if (disposing)
            {
                // Dispose managed resources.
                if (m_workerThread != null)
                {
                    m_workerThread.Abort();
                    m_workerThread = null;
                }
            }

            // Note disposing has been done.
            m_isDisposed = true;
        }

        
    }
}
