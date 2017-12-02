using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using Assets.Scripts.Other;
using Assets.Scripts.State;

namespace Assets.Scripts.Service
{
    public class Game : MonoBehaviour
    {
        public InputField IPAddress;
        public InputField Port;

        // Network
        public string serverIPAddress = "127.0.0.1";
        public int serverPort = 25001;
        public int maxConnectedPlayers = 8;
        public int connectedPlayerCount = 0;
        public bool isConnected = false;
        public IList<NetworkPlayer> connectedPlayers = new List<NetworkPlayer>();

        // UI
        public GameObject UIMain;
        public GameObject UIServer;
        public GameObject UIClient;

        // Service
        void Start()
        {
            UIMain.SetActive(true);
            UIServer.SetActive(false);
            UIClient.SetActive(false);
        }

        void Update()
        {
            serverIPAddress = IPAddress.text;
            serverPort = int.Parse(Port.text);
        }

        // Server
        void OnPlayerConnected(NetworkPlayer player)
        {
            connectedPlayers.Add(player);
            ++connectedPlayerCount;
            Debug.Log("Player " + connectedPlayerCount + " connected from " + player.ipAddress + ":" + player.port);
            isConnected = true;

            // Send server configs to clients
            gameObject.GetComponent<Server>().FirstSetupPlayer(player);
            Debug.Log("Sending server configurations to client");
        }
        
        // Server
        void OnPlayerDisconnected(NetworkPlayer player)
        {
            connectedPlayers.Remove(player);
            --connectedPlayerCount;
            GameObject.FindGameObjectWithTag("NumberOfClientText").GetComponent<Text>().text = connectedPlayerCount + "/" + maxConnectedPlayers;
            Debug.LogWarning("Clean up after player " + player);
            isConnected = false;
        }

        // Client
        void OnConnectedToServer()
        {
            UIMain.SetActive(false);
            UIServer.SetActive(false);
            UIClient.SetActive(true);
            Debug.LogWarning("Connected to server");
            gameObject.GetComponent<Client>().enabled = true;
            isConnected = true;
        }

        // Client
        void OnDisconnectedFromServer(NetworkDisconnection info)
        {
            UIMain.SetActive(true);
            UIServer.SetActive(false);
            UIClient.SetActive(false);


            if (Network.isServer)
            {
                Debug.LogWarning("Local server connection disconnected");
            }
            else
            {
                if (info == NetworkDisconnection.LostConnection)
                    Debug.LogWarning("Lost connection to the server");
                else
                {
                    Debug.LogWarning("Successfully diconnected from the server");
                }
            }
            gameObject.GetComponent<Client>().enabled = false;
            //Destroy(gameObject.GetComponent<Client>());
            isConnected = false;
        }

        // Client
        void OnFailedToConnect(NetworkConnectionError error)
        {
            UIMain.SetActive(true);
            UIServer.SetActive(false);
            UIClient.SetActive(false);
            Debug.LogWarning("Could not connect to server: " + error);
        }

        public void StartServer()
        {
            UIMain.SetActive(false);
            UIServer.SetActive(true);

            bool useNat = !Network.HavePublicAddress();
            Network.InitializeServer(32, serverPort, useNat);
            gameObject.GetComponent<Server>().enabled = true;
            //gameObject.AddComponent<Server>();

            Debug.LogWarning("Starting server");
        }

        public void EndServer()
        {
            UIMain.SetActive(true);
            UIServer.SetActive(false);

            //foreach (var player in connectedPlayer)
            //{
            //    Network.CloseConnection(new NetworkPlayer(player.ipAddress, player.port), true);
            //}

            Network.Disconnect(200);
            MasterServer.UnregisterHost();
            gameObject.GetComponent<Server>().enabled = false;
            //Destroy(gameObject.GetComponent<Server>());
            Debug.LogWarning("End server");
        }
        
        public void StartClient()
        {
            UIMain.SetActive(false);
            Network.Connect(serverIPAddress, serverPort);
            Debug.LogWarning("Starting client connecting to " + serverIPAddress + ":" + serverPort);
        }

        public void EndClient()
        {
            Network.Disconnect(200);
            //Network.CloseConnection(new NetworkPlayer(serverIPAddress, serverPort), true);
            Debug.LogWarning("End client");
        }
    }
}