using System;
using System.Collections.Concurrent;
using System.Threading;

namespace herst.threading
{
    public class SingleThreadWorkQueue<T>
    {
        public int QueueSize => Queue.Count;
        private readonly BlockingCollection<T> Queue = new();
        private readonly Thread workerThread;
        private readonly Action<T> Process;

        public SingleThreadWorkQueue(Action<T> process)
        {
            // setting workerThread.IsBackground = true;
            // makes it so when the main thread exits, workerThread will also exit
            // without that, workerThread will keep running and the program will hang
            Process = process;
            workerThread = new Thread(WorkLoop) { IsBackground = true };
            workerThread.Start();
        }

        public void Enqueue(T item) => Queue.Add(item);

        private void WorkLoop()
        {
            foreach (var item in Queue.GetConsumingEnumerable())
                Process.Invoke(item);
        }
    }
}