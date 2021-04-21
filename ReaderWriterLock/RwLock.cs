using System;
using System.Collections.Generic;
using System.Threading;

namespace ReaderWriterLock
{
    public class RwLock : IRwLock
    {
        private readonly object _writeLock = new object();
        private readonly List<object> _readLocks = new List<object>();

        public void ReadLocked(Action action)
        {
            var readLock = new object();

            lock (readLock)
            {
                lock (_writeLock)
                    _readLocks.Add(readLock);

                action.Invoke();
            }
        }

        public void WriteLocked(Action action)
        {
            lock (_writeLock)
            {
                _readLocks.ForEach(Monitor.Enter);
                action.Invoke();
                _readLocks.ForEach(Monitor.Exit);
                _readLocks.Clear();
            }
        }
    }
}