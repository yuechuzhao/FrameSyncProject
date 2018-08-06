from socketserver import BaseRequestHandler, UDPServer
from time import ctime, sleep
import threading
from enum import Enum

class UnitClient:
    def __init__(self, guid, addr):
        self.guid = guid
        self.address = addr
        self.msg_datas = []

    def record_msg(self, msg_data):
        self.msg_datas.append(msg_data)


class AppConst:
    HEART = -1
    SERVER_FRAME = 0
    CREATE = 1
    MOVE = 2

    FIRST_SPLIT = "|"
    SECOND_SPLIT = ";"
    THIRD_SPLIT = ","

    FuncMapping = {
        HEART: "on_heart_beat",
        CREATE: "on_create",
        MOVE: "on_move",
        SERVER_FRAME: "on_request_frame"
    }


class QueueManager:
    send_id = 0


# 帧控制器，在这里控制逻辑帧的步进
class FrameManager:
    # 每66毫秒逻辑帧前进一帧
    FRAME_MS = 66
    current_frame = 0

    def step_frame(self):
        while True:
            sleep(self.FRAME_MS / 1000)
            self.current_frame += 1

    def start_step(self):
        t = threading.Thread(target=self.step_frame)
        t.start()




class State(Enum):
    waiting = 0
    prepare = 1
    battle = 2
    settle = 3


class BattleManager:
    battle_id = 0
    is_in_battle = False
    is_in_prepare = False
    # 开始计时的时间
    start_time_count = 10

    def __init__(self):
        self.create_msgs = []

    # 开始游戏
    def start(self):
        if self.is_in_battle:
            return
        self.is_in_battle = True
        self.create_msgs.clear()
        client_manager.clear()
        # 倒计时

    def dispatch_receive_data(self, msg_data, send_func):
        # TODO 战中状态不能再添加新的角色了
        # TODO 准备状态不接受除添加新角色的任何消息

        # 处理一下data
        queue_manager.send_id += 1
        msg_data.SendId = queue_manager.send_id
        deal_func = getattr(self, AppConst.FuncMapping[msg_data.OperationCode])
        if not deal_func:
            return
        deal_func(msg_data, send_func)

    # TODO 心跳，如果连续不发送心跳的话就当做掉线
    def on_heart_beat(self, data, send_func):
        pass

    # 创建时，发给发送者：创建成功，以及当前所有其他玩家创建
    # 发给其他玩家：本角色创建成功
    def on_create(self, data, send_func):
        data.OperationInfoStr = str(client_manager.unit_id)
        sender = client_manager.get_client(data.Guid)
        self.create_msgs.append(data)
        send_func(sender.address, self.create_msgs)

        def send_to_client(client):
            if client.guid == data.Guid:
                return
            send_func(client.address, [data])

        client_manager.broadcast_to_clients(send_to_client)

    def on_move(self, data, send_func):
        self.send_all(data, send_func)

    def send_all(self, data, send_func):
        def send_to_client(client):
            send_func(client.address, [data])

        client_manager.broadcast_to_clients(send_to_client)

    def on_request_frame(self, data, send_func):
        sender = client_manager.get_client(data.Guid)
        send_func(sender.address, [data])


class ClientManager:
    clients = {}
    unit_id = 0

    def add_client(self, guid, addr):
        client = self.get_client(guid)
        if not client:
            client = UnitClient(guid, addr)
            self.clients[guid] = client
            self.unit_id += 1
        return client

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
            broadcast_func(client)

    # 清空所有的客户端
    def clear(self):
        self.clients.clear()


class ClientSyncData:
    def __init__(self, msg_str):
        params = msg_str.split(AppConst.SECOND_SPLIT)
        self.Guid = params[0]
        self.OperationCode = int(params[1])
        self.OperationInfoStr = params[2]
        self.FrameId = frame_manager.current_frame
        print(params)

    # FrameId = int.Parse(array[0]),
    # Guid = array[1],
    # OperationCode = int.Parse(array[2]),
    # SendId = int.Parse(array[3]),
    # OperationInfoStr = array[4]
    def parse(self):
        return AppConst.SECOND_SPLIT.join([str(self.FrameId), self.Guid,
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

        sender = client_manager.get_client(data.Guid)
        if not sender:
            sender = client_manager.add_client(data.Guid, self.client_address)

        # 转发给battlemanager处理

        def send_func(address, datas):
            send_msg = AppConst.FIRST_SPLIT.join([m.parse() for m in datas])
            send_msg = send_msg.encode()
            print("send msg", send_msg)
            sock.sendto(send_msg, address)

        battle_manager.dispatch_receive_data(data, send_func)

        # # 如果没有需要发的，就什么都不发
        # if len(msg_datas) == 0:
        #     return
        # client_manager.broadcast_to_clients(send_func)

        # for address in all_clients:
        #    for send_msg in send_msgs:
        #        send_msg = send_msg.encode()
        #        print("send msg", send_msg)
        #        sock.sendto(send_msg, address)


battle_manager = BattleManager()
queue_manager = QueueManager()
client_manager = ClientManager()
frame_manager = FrameManager()

if __name__ == '__main__':
    from socketserver import ThreadingUDPServer

    from test_package.demo import test_import
    test_import()
    frame_manager.start_step()
    server = ThreadingUDPServer(('', 20000), TimeHandler)
    server.serve_forever()
