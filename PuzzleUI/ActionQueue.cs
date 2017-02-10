using System;
using System.Collections.Concurrent;
using System.Threading;

namespace WpfApplication1
{
    public class ActionQueue
    {
        private BlockingCollection<Action> persisterQueue = new BlockingCollection<Action>();

        public ActionQueue()
        {
            var thread = new Thread(ProcessWorkQueue);
            thread.IsBackground = true;
            thread.Start();
        }

        private void ProcessWorkQueue()
        {
            while (true)
            {
                var nextWork = persisterQueue.Take();
                nextWork();
            }
        }

        public void Add(Action action)
        {
            persisterQueue.Add(action);
        }
    }
}
