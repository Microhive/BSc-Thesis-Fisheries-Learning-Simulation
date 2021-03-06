﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34209
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using Assets.Scripts.Other;
using Assets.Scripts.State;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Assets.Scripts.Service
{
    public class Client : MonoBehaviour
    {
        public InputField _playerName;
        public InputField MaxVessels;
        public InputField Vessels;
        public InputField Fished2;
        public InputField Fished3;
        public InputField Fished4;
        public InputField Fished5;
        public InputField Fished6;
        public InputField Fished7;
        public InputField Fished8;
        public InputField Fished9;

        public InputField Income;
        public InputField Cost;
        public InputField Funds;

        public Text CurrentYear;

        public ClientState configuration;

        void Start()
        {
            configuration = new ClientState();
        }

        void FixedUpdate()
        {
            CurrentYear.text = configuration.Year.ToString();
        }

        [RPC]
        void UpdateClient(byte[] config)
        {
            configuration = (ClientState)Converter.ByteArrayToObject(config);
            applyConfigurationToUI();
            Debug.LogWarning("Updating Client to reflect server results");

        }

        // Update based on server configurations
        [RPC]
        void ApplyServerConfigurationOnClient(byte[] _serverConfig, byte[] _clientConfig)
        {
            Debug.Log("Client received server configurations and client configuration for ");

            ServerState serverConfig = (ServerState)Converter.ByteArrayToObject(_serverConfig);
            ClientState clientConfig = (ClientState)Converter.ByteArrayToObject(_clientConfig);
            //Debug.LogWarning(client.playerName + ", Total fished: " + PlayerFishedByAge[2] + ", " + PlayerFishedByAge[3] + ", " + PlayerFishedByAge[4] + ", " + PlayerFishedByAge[5] + ", " + PlayerFishedByAge[6] + ", " + PlayerFishedByAge[7] + ", " + PlayerFishedByAge[8] + ", " + PlayerFishedByAge[9]);
            configuration.MaxfleetSize = serverConfig.MaxVessels;
            configuration.Year = serverConfig.CurrentYear;
            configuration.Fished = clientConfig.Fished;
            configuration.Revenue = clientConfig.Revenue;
            configuration.Expenses = clientConfig.Expenses;
            configuration.AccumulatedProfit = clientConfig.AccumulatedProfit;
            applyConfigurationToUI();
        }

        void applyConfigurationToUI()
        {
            _playerName.text = configuration.playerName;
            Vessels.text = configuration.FleetSize.ToString();
            MaxVessels.text = configuration.MaxfleetSize.ToString();
            CurrentYear.text = configuration.Year.ToString();

            Fished2.text = configuration.Fished[0].ToString();
            Fished3.text = configuration.Fished[1].ToString();
            Fished4.text = configuration.Fished[2].ToString();
            Fished5.text = configuration.Fished[3].ToString();
            Fished6.text = configuration.Fished[4].ToString();
            Fished7.text = configuration.Fished[5].ToString();
            Fished8.text = configuration.Fished[6].ToString();
            Fished9.text = configuration.Fished[7].ToString();
            Income.text = configuration.Revenue.ToString();
            Cost.text = configuration.Expenses.ToString();
            Funds.text = configuration.AccumulatedProfit.ToString();
        }

        void UpdateConfiguration()
        {
            configuration.playerName = _playerName.text;
            configuration.FleetSize = int.Parse(Vessels.text);
            configuration.IPAddress = Network.player.ipAddress;
            configuration.Port = Network.player.port;
            configuration.PlayerID = Network.player.ToString();

            configuration.fishingspots = new List<FishingSpot>();
            foreach (var item in GameObject.FindObjectsOfType<FishingSpotUI>())
            {
                FishingSpot spot = new FishingSpot();
                spot.FromDate = item.GetComponent<FishingSpotUI>().periodFrom.Value;
                spot.ToDate = item.GetComponent<FishingSpotUI>().periodTo.Value;
                spot.convertSpotToCoordinate(item.gameObject);
                configuration.fishingspots.Add(spot);
            }
        }

        [RPC]
        void SendConfigurationToServer()
        {
            Debug.LogWarning("Client: Sending Configuration");
            UpdateConfiguration();
            byte[] temp = Converter.ObjectToByteArray(configuration);
            gameObject.GetComponent<NetworkView>().RPC("StoreConfiguration", RPCMode.Server, new object[] { temp });
        }
    }
}