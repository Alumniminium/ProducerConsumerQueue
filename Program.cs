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
        public static readonly SingleThreadWorkQueue<string> BackgroundConsole = new (Console.WriteLine);
        public static readonly MultiThreadWorkQueue<string> BackgroundProxyTester = new (TestProxy, 24);

        public static void Main()
        {
            File.ReadAllLines("proxylist.txt").ToList().ForEach(BackgroundProxyTester.Enqueue);

            while(BackgroundProxyTester.QueueSize > 0)
            {
                BackgroundConsole.Enqueue($"[{Environment.CurrentManagedThreadId}]: Queue size: {BackgroundProxyTester.QueueSize}");
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
                BackgroundConsole.Enqueue($"[{Environment.CurrentManagedThreadId}]: {proxy} {(works ? "works" : "dead")}");
            }
        }
    }
}
