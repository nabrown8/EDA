from pythonosc.dispatcher import Dispatcher
from pythonosc.osc_server import BlockingOSCUDPServer
import csv
import time


def print_handler(address, *args):
    print(f"{address}: {args}")


def default_handler(address, *args):
    if address =="/EmotiBit/0/EDA":
        header = [time.time()]
        for edaVals in args:
            print(edaVals)
            header.append(edaVals)
            with open('EDAReadings.csv', 'a') as f:
                writer = csv.writer(f)
                writer.writerow(header)

    


dispatcher = Dispatcher()
dispatcher.map("/something/*", print_handler)
dispatcher.set_default_handler(default_handler)

ip = "127.0.0.1"
port = 12345

server = BlockingOSCUDPServer((ip, port), dispatcher)
server.serve_forever() 