using System;

namespace ConnectFlood
{
    public interface IAttack
    {
        void StartFlood(string target, int packetCount, int speed,string str);

    }
}
