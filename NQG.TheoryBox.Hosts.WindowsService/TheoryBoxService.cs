namespace NQG.TheoryBox.Host.WindowsService
{
    using Microsoft.Owin.Hosting;
    using Owin;
    using System;
    using Topshelf;

    public class TheoryBoxService : ServiceControl, IDisposable
    {
        private readonly object m_syncRoot = new Object();
        private bool m_isDisposed;

        private IDisposable m_theoryBoxWebApp;

        public bool Start(HostControl control)
        {
            m_theoryBoxWebApp = WebApp.Start("http://localhost:8888", app =>
            {
                app.UseTheoryBoxWeb();
            });

            return true;
        }

        public bool Stop(HostControl control)
        {
            DisposeApps();
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
                DisposeApps();
            }

            // Note disposing has been done.
            m_isDisposed = true;
        }

        private void DisposeApps()
        {
            if (m_theoryBoxWebApp != null)
            {
                lock (m_syncRoot)
                {
                    if (m_theoryBoxWebApp != null)
                    {
                        m_theoryBoxWebApp.Dispose();
                        m_theoryBoxWebApp = null;
                    }
                }
            }
        }
    }
}
