#!/bin/bash
echo "test"
echo "hello world!.."
echo "I know how to do this know, Linux is awesome"
#cygstart bin/Debug/EmpaticaBLEClient.exe
cygstart /cygdrive/c/Users/carlo/AppData/Local/Microsoft/WindowsApps/python3.9.exe -i dataserver.py & bin/Debug/EmpaticaBLEClient.exe && fg
#END
