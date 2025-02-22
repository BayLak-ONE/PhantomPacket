using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace ConnectFlood
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 3)
            {
                if (args.Length == 1 && (args[0].ToLower() == "help" || args[0] == "-help"))
                {
                    OpenHelpFile();
                    return;
                }
                Console.Title = "PhantomPacket";
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("                    | ");
                Console.WriteLine("____________    __ -+-  ____________ ");
                Console.WriteLine("\\_____     /   /_ \\ |   \\     _____/");
                Console.WriteLine(" \\_____    \\____/  \\____/    _____/");
                Console.WriteLine("  \\_____                    _____/");
                Console.WriteLine("     \\___________  ___________/ ");
                Console.WriteLine("               /____\\ PhantomPacket v1.0\n\n");
                Console.ResetColor();
                Console.WriteLine("Created by BayLak");
                Console.WriteLine("Github : https://github.com/BayLak-ONE/");
                Console.WriteLine("Donate BTC: bc1qctxfu3ar94gs3whjyrjl974dhe79wr6nex3txg\n\n");
                Console.WriteLine("Usage: PhantomPacket.exe help\n");
                Console.WriteLine("Usage: PhantomPacket.exe <Protocol> <Target> <PacketCount> [Speed]");
                return;
            }
            string protocol = args[0].ToUpper();
            string target = args[1];
            string none = args.Length > 4 ? args[4] : "";
            //Delete port from protocol
            if (protocol == "Not nedded")
            {
                target = target.Split(':')[0];
            }
            int packetCount = args[2].ToLower() == "auto" ? -1 : ParseInt(args[2]);
            int speed = args.Length > 3 ? ParseInt(args[3]) : 1000;
            Dictionary<string, IAttack> attackTypes = GetAttackTypes();

            if (attackTypes.ContainsKey(protocol))
            {
                try
                {
                    attackTypes[protocol].StartFlood(target, packetCount, speed ,none);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error occurred while starting flood: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine(
                    "Unsupported attack type! Available types: TCP, UDP, UCP, PING, NTP, HTTPV1, HTTPV2, SYN, IIPORT, IRPORT, ARP, DNS, SMB, SNMP, MEMCAHED, FTP, TELNET, CHARGEN, LDAP, CLDAP"
                );
            }
        }
        static int ParseInt(string input)
        {
            return int.TryParse(input, out int result) ? result : 0;
        }
        static Dictionary<string, IAttack> GetAttackTypes()
        {
            return new Dictionary<string, IAttack>
            {
                { "TCP", new TcpFlood() },
                { "TCPM", new TcpmFlood() },
                { "UDP", new UdpFlood() },
                { "UDPM", new UdpmFlood() },
                { "UCP", new UcpFlood() },
                { "PING", new PingFlood() },
                { "NTP", new NtpFlood() },
                { "HTTPV1", new HttpV1Flood() },
                { "HTTPV2", new HttpV2Flood() },
                { "HTTPV3", new HttpV3Flood() },
                { "SYN", new SynFlood() },
                { "IIPORT", new IIPortFlood() },
                { "IRPORT", new IRPortFlood() },
                { "ARP", new ArpFlood() },
                { "DNS", new DnsFlood() },
                { "SMB", new SmbFlood() },
                { "SNMP", new SnmpFlood() },
                { "SMTP", new SmtpFlood() },
                { "MEMCACHED", new MemcachedFlood() },
                { "FTP", new FtpFlood() },
                { "TELNET", new TelnetFlood() },
                { "CHARGEN", new ChargenFlood() },
                { "LDAP", new LdapFlood() },
                { "CLDAP", new CldapFlood() }
            };
        }
        static void OpenHelpFile()
        {
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("PhantomPacket.Resources.help.htm"))
            {
                if (stream == null)
                {
                    Console.WriteLine("Error: file not found.");
                    return;
                }
                using (StreamReader reader = new StreamReader(stream))
                {
                    string htmlContent = reader.ReadToEnd();
                    string tempFilePath = Path.Combine(Path.GetTempPath(), "help_temp.htm");
                    File.WriteAllText(tempFilePath, htmlContent);
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = tempFilePath,
                        UseShellExecute = true
                    });
                }
            }
        }
    }
}
