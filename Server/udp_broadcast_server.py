from socketserver import BaseRequestHandler, UDPServer


class UnitClient:
    def __init__(self, guid, addr):
        self.guid = guid
        self.address = addr


class QueueManager:
    send_id = 0


class ClientManager:
    clients = {}
    unit_id = 0

    def add_client(self, guid, addr):
        client = self.get_client(guid)
        if not client:
            client = UnitClient(guid, addr)
            self.clients[guid] = client
            self.unit_id += 1

    def get_client(self, guid):
        for client_guid, client in self.clients.items():
            if client_guid == guid:
                return client
        return None

    def remove_client(self, guid):
        for client_guid in self.clients.keys():
            if client_guid == guid:
                self.clients.pop(client_guid)

    def broadcast_to_clients(self, broadcast_func):
        for client in self.clients.values():
            broadcast_func(client.address)


queue_manager = QueueManager()
client_manager = ClientManager()

second_spliter = ";"
first_spliter = "|"


class ClientSyncData:
    SendId = 0

    def __init__(self, msg_str):
        params = msg_str.split(second_spliter)
        self.FrameId = int(params[0])
        self.Guid = params[1]
        self.OperationCode = int(params[2])
        self.OperationInfoStr = params[3]
        print(params)

    # FrameId = int.Parse(array[0]),
    # Guid = array[1],
    # OperationCode = int.Parse(array[2]),
    # SendId = int.Parse(array[3]),
    # OperationInfoStr = array[4]
    def parse(self):
        return second_spliter.join([str(self.FrameId), self.Guid,
                         str(self.OperationCode), str(self.SendId),
                         self.OperationInfoStr])


class TimeHandler(BaseRequestHandler):
    # 发送序列
    send_id = 0
    unit_id = 0
    # 当前帧
    current_frame = 0
    create_msgs = []

    def handle(self):
        print('Got connection from', self.client_address)
        # Get message and client socket
        msg, sock = self.request
        # 这里还缺检测socket是否已经关闭的逻辑
        print('client msg', msg.decode("ascii"))
        data = ClientSyncData(msg.decode("ascii"))

        if not client_manager.get_client(data.Guid):
            client_manager.add_client(data.Guid, self.client_address)

        send_msgs = self.deal_msg_data(data)

        def send_func(addr):
            send_msg = first_spliter.join(send_msgs)
            send_msg = send_msg.encode()
            print("send msg", send_msg)
            sock.sendto(send_msg, addr)

        client_manager.broadcast_to_clients(send_func)

        #for address in all_clients:
        #    for send_msg in send_msgs:
        #        send_msg = send_msg.encode()
        #        print("send msg", send_msg)
        #        sock.sendto(send_msg, address)

    def deal_msg_data(self, data):
        queue_manager.send_id += 1
        data.SendId = queue_manager.send_id
        print("deal msg, operationInfoStr ", data.OperationInfoStr, "send_id", self.send_id)
        if data.OperationCode == 1:
            data.OperationInfoStr = str(client_manager.unit_id)
            self.create_msgs.append(data.parse())
            print("new creation", len(self.create_msgs))
            return self.create_msgs
        if data.OperationCode == 2:
            pass

        return [data.parse()]


if __name__ == '__main__':
    from socketserver import ThreadingUDPServer

    server = ThreadingUDPServer(('', 20000), TimeHandler)
    server.serve_forever()
