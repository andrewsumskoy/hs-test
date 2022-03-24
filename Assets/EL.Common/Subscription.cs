using System;

namespace EL.Common
{
    public class Subscription : IDisposable
    {
        private Action _do;

        public Subscription(Action action)
        {
            _do = action;
        }

        public void Dispose()
        {
            if (_do != null)
            {
                _do();
                _do = null;
            }
        }
    }
}