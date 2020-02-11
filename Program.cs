using System;
using System.Diagnostics;
using System.Threading;

namespace PCQ
{
    public static class Program
    {
        public static bool USE_QUEUED_OUTPUT,RedDone,BlueDone,GreenDone,WhiteDone = false;

        public static Thread WriteBlueThread, WriteRedThread, WriteGreenThread,WriteWhiteThread;
        public static ManualResetEvent Block = new ManualResetEvent(false);

        public static void Main()
        {
            SafeConsole.Clear();
            Block.Reset();
            RedDone = false;
            BlueDone = false;
            WhiteDone = false;
            GreenDone = false;

            SafeConsole.WriteLine("USE QUEUE? (Y/N)");
            var input = Console.ReadLine().ToUpperInvariant();
            switch (input)
            {
                case "Y":
                    USE_QUEUED_OUTPUT = true;
                    break;
                case "N":
                    USE_QUEUED_OUTPUT = false;
                    break;
                default:
                    Main();
                    break;
            }
            
            WriteBlueThread = new Thread(WriteBlueLoop);
            WriteRedThread = new Thread(WriteRedLoop);
            WriteGreenThread = new Thread(WriteGreenLoop);
            WriteWhiteThread = new Thread(WriteWhiteLoop);

            WriteBlueThread.Start();
            WriteRedThread.Start();
            WriteGreenThread.Start();
            WriteWhiteThread.Start();
            Block.Set();


            while (!RedDone || !BlueDone || !WhiteDone || !GreenDone)
                Thread.Sleep(1);

            if (USE_QUEUED_OUTPUT)
                Thread.Sleep(100);
            
            SafeConsole.WriteLine("Simulation finished. Press ENTER to restart.");
            Console.ReadLine();
            Main();
        }

        public static void WriteBlueLoop()
        {
            Block.WaitOne();
            for (int i = 0; i < 5; i++)
            {
                SafeConsole.WriteLine("BLUE", ConsoleColor.Blue, USE_QUEUED_OUTPUT);
            }
            BlueDone = true;
        }

        public static void WriteRedLoop()
        {
            Block.WaitOne();
            for (int i = 0; i < 5; i++)
            {
                SafeConsole.WriteLine("RED", ConsoleColor.Red, USE_QUEUED_OUTPUT);
            }
            RedDone = true;
        }
        public static void WriteGreenLoop()
        {
            Block.WaitOne();
            for (int i = 0; i < 5; i++)
            {
                SafeConsole.WriteLine("GREEN", ConsoleColor.Green, USE_QUEUED_OUTPUT);
            }
            GreenDone = true;
        }

        public static void WriteWhiteLoop()
        {
            Block.WaitOne();
            for (int i = 0; i < 5; i++)
            {
                SafeConsole.WriteLine("WHITE", ConsoleColor.White, USE_QUEUED_OUTPUT);
            }
            WhiteDone = true;
        }
    }
}
