from socketserver import BaseRequestHandler, UDPServer

all_clients = []


class UnitClient:
    def __init__(self, guid, addr):
        self.guid = guid
        self.address = addr


class ClientSyncData:
    SendId = 0

    def __init__(self, msg_str):
        params = msg_str.split("|")
        self.FrameId = int(params[0])
        self.Guid = params[1]
        self.OperationCode = int(params[2])
        self.OperationInfoStr = params[3]

    #FrameId = int.Parse(array[0]),
    #Guid = array[1],
    #OperationCode = int.Parse(array[2]),
    #SendId = int.Parse(array[3]),
    #OperationInfoStr = array[4]
    def parse(self):
        return "|".join([str(self.FrameId), self.Guid,
                         str(self.OperationCode), str(self.SendId),
                         self.OperationInfoStr])


class TimeHandler(BaseRequestHandler):
    # 发送序列
    send_id = 0
    unit_id = 0
    #当前帧
    current_frame = 0

    def handle(self):
        print('Got connection from', self.client_address)
        # Get message and client socket
        msg, sock = self.request
        print('client msg', msg.decode("ascii"))
        if self.client_address in all_clients:
            return

        all_clients.append(self.client_address)
        send_msg = self.deal_msg(msg)

        for addr in all_clients:
            sock.sendto(send_msg.encode(), addr)

    def deal_msg(self, msg):
        data = ClientSyncData(msg.decode("ascii"))
        self.send_id += 1
        data.FrameId += 1
        data.SendId = self.send_id
        if data.OperationCode == 1:
            self.unit_id += 1
            data.OperationInfoStr = str(self.unit_id)
        print("send msg", data.parse())
        return data.parse()

if __name__ == '__main__':
    from socketserver import ThreadingUDPServer
    serv = ThreadingUDPServer(('', 20000), TimeHandler)
    serv.serve_forever()
