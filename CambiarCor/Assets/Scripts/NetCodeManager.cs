using Unity.Netcode;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace NetCode
{
    public class NetCodeManager : MonoBehaviour
    {
        public List<Material> materials = new List<Material>();
        public static NetCodeManager instance;

        private List<int> materialsUsed = new List<int>();

        void Awake() {
            instance = this;
        }

        void OnGUI()
        {
            GUILayout.BeginArea(new Rect(10, 10, 300, 300));
            if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
            {
                StartButtons();
            }
            else
            {
                StatusLabels();

                if (NetworkManager.Singleton.IsClient) {

                    SubmitNewColor();
                }
            }

            GUILayout.EndArea();
        }

        static void StartButtons()
        {            
            if (GUILayout.Button("Host")) NetworkManager.Singleton.StartHost();
            if (GUILayout.Button("Client")) NetworkManager.Singleton.StartClient();
            if (GUILayout.Button("Server")) NetworkManager.Singleton.StartServer();
        }

        static void StatusLabels()
        {
            var mode = NetworkManager.Singleton.IsHost ?
                "Host" : NetworkManager.Singleton.IsServer ? "Server" : "Client";

            GUILayout.Label("Transport: " +
                NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetType().Name);
            GUILayout.Label("Mode: " + mode);
        }

        static void SubmitNewColor()
        {
            if (GUILayout.Button(NetworkManager.Singleton.IsServer ? "" : "Request Color Change"))
            {
                if (NetworkManager.Singleton.IsClient )
                {
                    var playerObject = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();
                    var player = playerObject.GetComponent<NetCodePlayer>();
                    player.ChangeColor();
                }
            }
        }

        void Update() {
            if (NetworkManager.Singleton.IsServer) {
                if (NetworkManager.Singleton.ConnectedClientsIds.Count > 6) {
                    NetworkManager.Singleton.DisconnectClient(NetworkManager.Singleton.ConnectedClientsIds[NetworkManager.Singleton.ConnectedClientsIds.Count - 1]);
                }

                materialsUsed.Clear();
                
                foreach (ulong uid in NetworkManager.Singleton.ConnectedClientsIds)
                    materialsUsed.Add(NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(uid).GetComponent<NetCodePlayer>().Color.Value);

                foreach (ulong uid in NetworkManager.Singleton.ConnectedClientsIds)
                    NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(uid).GetComponent<NetCodePlayer>().materialsUsed = materialsUsed;
            }
        }
    }
}