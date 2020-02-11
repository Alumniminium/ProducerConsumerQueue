using System;
using System.Collections.Generic;
using System.Text;

namespace PCQ
{
    public static class SafeConsole
    {
        public static void WriteLine(object content, ConsoleColor color = ConsoleColor.White, bool useQueue = false)
        {
            if (useQueue)
            {
                var pair = (content, color);
                IOQueue.Instance.Enqueue(pair);
            }
            else
            {
                Console.ForegroundColor = color;
                Console.WriteLine(content);
                Console.ResetColor();
            }
        }

        public static void Clear()
        {
            lock (Console.Out)
            {
                Console.Clear();
            }
        }
    }
}
