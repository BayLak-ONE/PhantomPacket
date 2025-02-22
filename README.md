# PhantomPacket
PhantomPacket is a DDOS Attack tool used to send data packets intensively to a specific target, aiming to consum
![Image](https://github.com/user-attachments/assets/94be7a6f-9cd5-4e74-b342-06adf519f2d5)

[![Download](https://img.shields.io/badge/Download-PhantomPacket-blue?style=for-the-badge&logo=windows)](https://github.com/BayLak-ONE/PhantomPacket/raw/refs/heads/main/PhantomPacket/bin/Debug/PhantomPacket.rar)


This simple tool was created similar to hping3, but simple. I created it for educational purposes only. I do not bear any responsibility for misuse of the tool. It is an open source , language support C#.

**Usage:**
```cmd
PhantomPacket.exe <Protocol> <Target> <PacketCount> [Speed]
```

**Examples:**
1. **Send an infinite number of PING packets to an IP at speed 10:**
   ```cmd
   PhantomPacket.exe PING 192.168.1.1:80 auto 10
   ```

2. **Send 16 TCP packets to an IP at speed 10:**
   ```cmd
   PhantomPacket.exe TCP 192.168.1.1:80 16 10
   ```

3. **Send an infinite number of TCP packets to a domain at speed 10:**
   ```cmd
   PhantomPacket.exe TCP domain.com auto 10
   ```

---

**Parameter Explanation:**
- `<Protocol>`: The type of packet to send (e.g., `PING`, `TCP`, `UDP`, etc.).
- `<Target>`: The target IP or domain, with an optional port (e.g., `192.168.1.1:80` or `domain.com`).
- `<PacketCount>`: Number of packets to send (`auto` means infinite).
- `[Speed]` (optional): The speed at which packets are sent.

---

**Help Command:**
To display usage instructions, use one of the following commands:
```cmd
PhantomPacket.exe -help
```
or
```cmd
PhantomPacket.exe help
```

