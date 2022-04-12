using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;

namespace herst.threading
{
    public static class Program
    {
        public static readonly MultiThreadWorkQueue<string> gbConsole = new (threadCount: 1, @do: Console.WriteLine);
        public static readonly MultiThreadWorkQueue<string> bgProxyTester = new (threadCount: 24, @do: TestProxy);

        public static void Main()
        {
            File.ReadAllLines("proxylist.txt").ToList().ForEach(bgProxyTester.Enqueue);

            while(bgProxyTester.QueueSize > 0)
            {
                gbConsole.Enqueue($"[{Environment.CurrentManagedThreadId}]: Queue size: {bgProxyTester.QueueSize}");
                Thread.Sleep(10000);
            }
        }

        private static void TestProxy(string proxy)
        {
            var match = Regex.Match(proxy, @"\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}:\d{1,5}", RegexOptions.Compiled);
            if (!match.Success)
                return;

            var ip = proxy.Split(':')[0];
            var port = ushort.Parse(proxy.Split(':')[1]);
            var works = false;

            var webProxy = new WebProxy(ip, port);
            var request = WebRequest.Create("https://her.st/404.html") as HttpWebRequest;
            request.Proxy = webProxy;
            request.Timeout = (int)TimeSpan.FromSeconds(10).TotalMilliseconds;

            try
            {
                request.GetResponse().GetResponseStream().CopyTo(Stream.Null);
                works=true;
            }
            catch {}
            finally
            {
                gbConsole.Enqueue($"[{Environment.CurrentManagedThreadId}]: {proxy} {(works ? "works" : "dead")}");
            }
        }
    }
}
