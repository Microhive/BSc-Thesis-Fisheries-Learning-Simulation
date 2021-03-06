//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34209
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using Assets.Scripts.Other;
using Assets.Scripts.Simulation;
using Assets.Scripts.State;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Service
{
    public class Server : MonoBehaviour
	{
        public Text CurrentYear;
        public Text NumberOfClient;
        public Text PlayerNameList;
        public InputField MaxVessels;
        public InputField MaintenanceFee;
        public InputField OilPrice;
        public InputField Recruitment;
        public InputField MarketValue;

        public InputField Age2;
        public InputField Age3;
        public InputField Age4;
        public InputField Age5;
        public InputField Age6;
        public InputField Age7;
        public InputField Age8;
        public InputField Age9;
        public InputField Age10;
        public InputField TotalAmount;
        public InputField TotalWeight;
        public InputField SpawnAmount;
        public InputField SpawnWeight;

        public ServerState configuration;
        private IList<NetworkPlayer> getPlayers() { return gameObject.GetComponent<Game>().connectedPlayers; }
        public IList<ClientState> readyPlayers { get; set; }
        private bool isConnected() { return gameObject.GetComponent<Game>().isConnected; }
        private bool waitingForClientConfigurations = false;
        private bool cancelSimulation = false;
        private int numberOfPlayersConfigsRequested = 0;
        public DateTime StartTime;

        void Start()
        {
            configuration = new ServerState();
            readyPlayers = new List<ClientState>();
            configuration.CurrentYear = 2000;
            configuration.MaxVessels = 120;
            StartTime = DateTime.Now;
        }

        void FixedUpdate()
        {
            NumberOfClient.text = getPlayers().Count + "/" + 8;

            string playerNamesText = "";

            // Printing list of players
            foreach (var player in getPlayers())
            {
                ClientState temp;
                if (gameObject.GetComponent<ServerStateList>().clientConfigs.TryGetValue(new Tuple<string, int, int>(player.ipAddress, player.port, configuration.CurrentYear + 1), out temp))
                {
                    playerNamesText += configuration.CurrentYear + ": " + temp.playerName + "\r\n";
                }
            }

            foreach (var player in readyPlayers)
            {
                //ClientConfiguration temp;

                //if (gameObject.GetComponent<ServerStateList>().clientConfigs.TryGetValue(new Tuple<string, int, uint>(player.IPAddress, player.Port, configuration.CurrentYear), out temp))
                //{
                //    playerNamesText += player.playerName + "\r\n";
                //}

                playerNamesText += player.playerName + "\r\n";
            }

            PlayerNameList.text = "Connected Players:" + "\r\n" + playerNamesText;

            if (waitingForClientConfigurations)
            {
                if (readyPlayers.Count == getPlayers().Count)
                {
                    Debug.LogWarning("All configurations have arrived!");
                    cancelSimulation = false;
                    waitingForClientConfigurations = false;
                    RunSimulation();
                }
                else if (numberOfPlayersConfigsRequested != getPlayers().Count)
                {
                    Debug.LogError("Player disconnected! Abording simulation!");
                    cancelSimulation = true;
                }
            }

            if (cancelSimulation)
            {
                waitingForClientConfigurations = false;
                cancelSimulation = false;
            }
        }

        public void StartSimulationByGettingClientConfigurations()
        {
            // Clear to store configurations
            readyPlayers = new List<ClientState>();
            Debug.LogWarning("Checking if client configurations have arrived! " + readyPlayers.Count);

            // Fetch configurations
            Debug.LogWarning("Server: Getting Configuration");
            GetPlayersConfigurations();
            numberOfPlayersConfigsRequested = getPlayers().Count;
            waitingForClientConfigurations = true;
        }

        public void RunSimulation()
        {
            // Modify configurations
            CurrentYear.text = (++configuration.CurrentYear).ToString();
            ReflectUIChangeToConfiguration();

            // Save configurations
            gameObject.GetComponent<Assets.Scripts.State.ServerStateList>().serverConfigs.Add(configuration.CurrentYear, configuration);
            int sumOfSpots = 0;
            foreach (var conf in readyPlayers)
            {
                sumOfSpots += conf.fishingspots.Count;
                gameObject.GetComponent<Assets.Scripts.State.ServerStateList>().clientConfigs.Add(new Tuple<string, int, int>(conf.IPAddress, conf.Port, configuration.CurrentYear), conf);
            }
            Debug.LogWarning(sumOfSpots);

            // Start simulation
            Debug.LogWarning("Starting Simulation! " + readyPlayers.Count);

            gameObject.GetComponent<Simulator>().RunSimulation(configuration.CurrentYear, readyPlayers);

            YearResult temp;
            gameObject.GetComponent<Simulator>().Results.TryGetValue(configuration.CurrentYear, out temp);
            Debug.LogWarning("Currently: " + temp.AgeGroup[2] + ", " + temp.AgeGroup[3] + ", " + temp.AgeGroup[4] + ", " + temp.AgeGroup[5] + ", " + temp.AgeGroup[6] + ", " + temp.AgeGroup[7] + ", " + temp.AgeGroup[8] + ", " + temp.AgeGroup[9] + ", " + temp.AgeGroup[10]);
            Debug.LogWarning("TotalFished: " + temp.AgeGroupFished[2] + ", " + temp.AgeGroupFished[3] + ", " + temp.AgeGroupFished[4] + ", " + temp.AgeGroupFished[5] + ", " + temp.AgeGroupFished[6] + ", " + temp.AgeGroupFished[7] + ", " + temp.AgeGroupFished[8] + ", " + temp.AgeGroupFished[9]);
            Debug.LogWarning("OtherStats: " + temp.TotalAmount + ", " + temp.TotalSpawnAmount + ", " + temp.TotalSpawnWeight + ", " + temp.TotalWeight);
            Debug.LogWarning("Reflecting result to clients!");

            UpdateStockStatisticsUI(temp);
            UpdatePlayers();
        }

        private void UpdateStockStatisticsUI(YearResult temp)
        {
            Age2.text = temp.AgeGroup[2].ToString();
            Age3.text = temp.AgeGroup[3].ToString();
            Age4.text = temp.AgeGroup[4].ToString();
            Age5.text = temp.AgeGroup[5].ToString();
            Age6.text = temp.AgeGroup[6].ToString();
            Age7.text = temp.AgeGroup[7].ToString();
            Age8.text = temp.AgeGroup[8].ToString();
            Age9.text = temp.AgeGroup[9].ToString();
            Age10.text = temp.AgeGroup[10].ToString();
            TotalAmount.text = temp.TotalAmount.ToString();
            TotalWeight.text = temp.TotalWeight.ToString();
            SpawnAmount.text = temp.TotalSpawnAmount.ToString();
            SpawnWeight.text = temp.TotalSpawnWeight.ToString();
        }

        private void GetPlayersConfigurations()
        {
            foreach (var player in getPlayers())
            {
                GetPlayerConfiguration(player);
            }
        }

        private bool validateInput()
        {
            return true;
        }

        private void UpdatePlayers()
        {
            foreach (var player in getPlayers())
            {
                UpdatePlayer(player);
            }
        }

        [RPC]
        void StoreConfiguration(byte[] config)
        {
            ClientState temp = (ClientState)Converter.ByteArrayToObject(config);
            Debug.LogWarning("Recieving player: " + temp.playerName);
            readyPlayers.Add(temp);
        }

        private void GetPlayerConfiguration(NetworkPlayer player)
        {
            Debug.LogWarning(player.ToString() + " sending");
            gameObject.GetComponent<NetworkView>().RPC("SendConfigurationToServer", player);
        }

        public void FirstSetupPlayer(NetworkPlayer player)
        {
            // Should also give the person a name
            UpdatePlayer(player);
        }

        public void UpdatePlayer(NetworkPlayer player)
        {
            // Find relevant clientconfiguration
            ClientState temp = GetClientConfigurationBasedOnNetworkPlayer(player);

            // Else start anew
            if (temp == null)
            {
                temp = new ClientState();
                temp.PlayerID = player.ToString();
            }

            Debug.LogWarning(temp.playerName);

            // Send configuration
            gameObject.GetComponent<NetworkView>().RPC("ApplyServerConfigurationOnClient", player, new object[] { Converter.ObjectToByteArray(configuration), Converter.ObjectToByteArray(temp) });
        }

        public ClientState GetClientConfigurationBasedOnNetworkPlayer(NetworkPlayer player)
        {
            // Find relevant clientconfiguration
            foreach (var item in readyPlayers)
                if (player.ToString() == item.PlayerID)
                    return item;

            return null;
        }

        void OnGUI()
        {
            if (isConnected())
            {
                //toggleShow = GUI.Toggle(new Rect(screenPosition.x, screenPosition.y, 40, 20), toggleShow, "�ki");
                //GUI.color = Color.red;
                //if (GUI.Button(new Rect(screenPosition.x + 80, screenPosition.y, 20, 20), "X"))
                //{
                //    Destroy(gameObject, 0.0f);
                //}
                //GUI.color = Color.white;
                //GUI.Label(new Rect(screenPosition.x, screenPosition.y + 20, 50, 20), "Fr�");
                //GUI.Label(new Rect(screenPosition.x, screenPosition.y + 40, 200, 20), "Til");
                //GUI.color = periodFrom.HasValue ? Color.white : Color.red;
                //stringDatePeriodFrom = GUI.TextField(new Rect(screenPosition.x + 30, screenPosition.y + 20, 70, 20), stringDatePeriodFrom, 5);
                //GUI.color = periodTo.HasValue ? Color.white : Color.red;
                //stringDatePeriodTo = GUI.TextField(new Rect(screenPosition.x + 30, screenPosition.y + 40, 70, 20), stringDatePeriodTo, 5);
            }
        }

        public void ReflectUIChangeToConfiguration()
        {
            configuration.MaintenanceFee = int.Parse(MaintenanceFee.text);
            configuration.MarketValue = int.Parse(MarketValue.text);
            configuration.MaxVessels = int.Parse(MaxVessels.text);
            configuration.OilPrice = int.Parse(OilPrice.text);
            configuration.Recruitment = long.Parse(Recruitment.text);
        }
    }
}
