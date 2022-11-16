#!/bin/bash
echo "Data Collection Starting"
cygstart /cygdrive/c/Users/carlo/AppData/Local/Microsoft/WindowsApps/python3.9.exe -i dataserver.py &
cygstart bin/Debug/EmpaticaBLEClient.exe &
#END
