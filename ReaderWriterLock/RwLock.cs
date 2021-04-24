using System;
using static System.Threading.Interlocked;

namespace ReaderWriterLock
{
    public class ReferenceBool
    {
        public bool Value;

        public ReferenceBool(bool value)
        {
            Value = value;
        }
    }

    public class RwLock : IRwLock
    {
        private static readonly ReferenceBool False = new (false);

        private static readonly ReferenceBool True = new (true);

        private ReferenceBool _writing = False;

        public void ReadLocked(Action action)
        {
            if (CompareExchange(ref _writing, False, False) == False)
                action.Invoke();
        }

        public void WriteLocked(Action action)
        {
            if (CompareExchange(ref _writing, True, False) == False)
                action.Invoke();
        }
    }
}
