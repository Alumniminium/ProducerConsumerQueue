using System;
using System.Collections.Concurrent;
using System.Threading;

namespace herst.threading
{
    public class MultiThreadWorkQueue<T>
    {
        public int QueueSize => Queue.Count;
        private readonly BlockingCollection<T> Queue = new();
        private readonly Thread[] workerThreads;
        private readonly Action<T> Process;

        public MultiThreadWorkQueue(Action<T> process, int threadCount = 2)
        {
            // setting workerThread.IsBackground = true;
            // makes it so when the main thread exits, workerThread will also exit
            // without that, workerThread will keep running and the program will hang
            Process = process;
            workerThreads = new Thread[threadCount];
            for(var i = 0; i < threadCount; i++)
            {
                workerThreads[i] = new Thread(WorkLoop) { IsBackground = true };
                workerThreads[i].Start();
            }
        }

        public void Enqueue(T item) => Queue.Add(item);

        private void WorkLoop()
        {
            foreach (var item in Queue.GetConsumingEnumerable())
                Process.Invoke(item);
        }
    }
}