using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NewcamdLogViewer
{
    class Program
    {
        private const int Port = 514;

        static void Main(string[] args)
        {
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            Task task = Task.Factory.StartNew(t => StartServer(tokenSource.Token), tokenSource.Token, tokenSource.Token);

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
            tokenSource.Cancel();
            if (task.IsCompleted)
            {
                task.Wait();
            }
        }

        private static void StartServer(CancellationToken token)
        {
            UdpClient listener = new UdpClient(Port);
            IPEndPoint groupEp = new IPEndPoint(IPAddress.Any, Port);

            try
            {
                using (listener)
                {
                    while (!token.IsCancellationRequested)
                    {
                        byte[] bytes = listener.Receive(ref groupEp);
                        Console.Write(Encoding.ASCII.GetString(bytes, 0, bytes.Length));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
