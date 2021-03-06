//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34209
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.State
{
    [Serializable]
	public class ClientState
	{
        public string IPAddress;
        public int Port;
        public string PlayerID;
        [SerializeField]
        public IList<FishingSpot> fishingspots = new List<FishingSpot>();
        public int FleetSize = 0;
        public int MaxfleetSize = 0;
        public string playerName = "";
        public int Year;
        public double[] Fished;
        public DateTime connectedTime;

        // Economics
        public double Revenue = 0;
        public double Expenses = 0;
        public double AccumulatedProfit = 0;
    }
}

