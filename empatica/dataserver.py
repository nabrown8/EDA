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
            with open('EmotiBit' + str(startTime) + '.csv', 'a') as f:
                writer = csv.writer(f)
                for x in header:
                    if x == 0: break
                    uniqStamp = [header[0], header[x]]
                    writer.writerow(uniqStamp)

def setup_file():
        global startTime
        startTime = time.time()
        try:
            with open('EmotiBit' + str(startTime) + '.csv', 'x') as f:
                writer = csv.writer(f)
                writer.writerow(['Timestamp', 'EDA'])
        except:
            print("File already exists.")


dispatcher = Dispatcher()
setup_file()
dispatcher.map("/something/*", print_handler)
dispatcher.set_default_handler(default_handler)

ip = "127.0.0.1"
port = 12345

server = BlockingOSCUDPServer((ip, port), dispatcher)
server.serve_forever() 