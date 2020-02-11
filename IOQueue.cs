using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text;
using System.Threading;

namespace PCQ
{
    public class IOQueue
    {
        private static IOQueue _Instance;
        public static IOQueue Instance => _Instance ?? (_Instance = new IOQueue());

        private ConcurrentQueue<(object output, ConsoleColor color)> _Queue = new ConcurrentQueue<(object, ConsoleColor)>();
        private Thread _WorkerThread;
        private volatile bool _KeepRunning = true;

        private AutoResetEvent Block = new AutoResetEvent(false);
        
        public IOQueue()
        {
            _WorkerThread = new Thread(WorkLoop)
            {
                IsBackground = true
            };
            _WorkerThread.Start();
        }

        public void Enqueue((object content, ConsoleColor color) pair)
        {
            _Queue.Enqueue(pair);
            Block.Set();
        }

        private void WorkLoop()
        {
            while (_KeepRunning)
            {
                Block.WaitOne();

                while (_Queue.TryDequeue(out var pair))
                {
                    var output = pair.output.ToString();
                    var color = pair.color;
                    
                    lock (Console.Out)
                    {
                        Console.ForegroundColor = color;
                        Console.WriteLine(output);
                        Console.ResetColor();
                    }
                }
            }
        }
    }
}
