from socketserver import BaseRequestHandler, UDPServer
import time


all_clients = []

class TimeHandler(BaseRequestHandler):

    def handle(self):
        print('Got connection from', self.client_address)
        # Get message and client socket
        msg, sock = self.request
        if self.client_address not in all_clients:
        	all_clients.append(self.client_address)
        for addr in all_clients:
        	sock.sendto(msg, addr)

if __name__ == '__main__':
    serv = UDPServer(('', 21003), TimeHandler)
    serv.serve_forever()