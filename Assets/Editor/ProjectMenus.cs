using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.UDPClient;
using UnityEditor;
using UnityEngine;

public class ProjectMenus {

    [MenuItem("Custom/修改服务器设置")]
    static void SetServerIP() {
        EditorWindow.GetWindow<ServerSettingWindow>("服务器设置", true);
    }
}

public class ServerSettingWindow : EditorWindow {

    private string _serverIp;
    private int _serverPort = -1;
    private int _localPort = -1;



    void OnGUI() {
        GUILayout.BeginVertical();
        if (string.IsNullOrEmpty(_serverIp)) {
            _serverIp = UDPSetting.ServerIP;
        }
        if (_serverPort == -1) {
            _serverPort = UDPSetting.ServerPort;
        }
        if (_localPort == -1) {
            _localPort = UDPSetting.LocalPort;
        }
        _serverIp = EditorGUILayout.TextField("服务器ip", _serverIp);
        _serverPort = EditorGUILayout.IntField("服务器端口", _serverPort);
        _localPort = EditorGUILayout.IntField("本地端口", _localPort);
        if (GUILayout.Button("保存")) {
            UDPSetting.ServerIP = _serverIp;
            UDPSetting.ServerPort = _serverPort;
            UDPSetting.LocalPort = _localPort;
        }
        GUILayout.EndHorizontal();
    }

}
