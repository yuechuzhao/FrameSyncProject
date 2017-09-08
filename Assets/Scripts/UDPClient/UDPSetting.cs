using UnityEngine;

namespace Assets.Scripts.UDPClient {
    public class UDPSetting {
        public static string ServerIP {
            get { return PlayerPrefs.GetString("UDPServerIP", "127.0.0.1"); }
            set {
                PlayerPrefs.SetString("UDPServerIP", value);
            }
        }

        public static int ServerPort {
            get { return PlayerPrefs.GetInt("ServerPort", 20000); }
            set {
                PlayerPrefs.SetInt("ServerPort", value);
            } 
        }

        public static int LocalPort {
            get { return PlayerPrefs.GetInt("LocalPort", 11000); }
            set {
                PlayerPrefs.SetInt("LocalPort", value);
            } 
        }
    }
}