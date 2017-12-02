using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.State;
using Assets.Scripts.Service;
using System.Linq;

namespace Assets.Scripts.Simulation
{
    public class Simulator : MonoBehaviour
    {
        public Transform topLeft;
        public Transform topRight;
        public Transform bottomLeft;
        public Transform bottomRight;
        public Vector2 coordinateTopLeft;
        public Vector2 coordinateBottomRight;
        public IList<int> ListOfClientsID;

        public IDictionary<int, YearResult> Results = new Dictionary<int, YearResult>();

        // Use this for initialization
        void Start()
        {
            IDictionary<int, double> ageGroups = new Dictionary<int, double>();
            ageGroups.Add(2, 22.221581);
            ageGroups.Add(3, 14.901450);
            ageGroups.Add(4, 9.720289);
            ageGroups.Add(5, 6.215929);
            ageGroups.Add(6, 3.974240);
            ageGroups.Add(7, 2.563200);
            ageGroups.Add(8, 1.660171);
            ageGroups.Add(9, 1.142986);
            ageGroups.Add(10, 0.893679);
            Results.Add(1999, new YearResult(1999, ageGroups));

            IDictionary<int, double> ageGroups2 = new Dictionary<int, double>();
            ageGroups2.Add(2, 22.217621);
            ageGroups2.Add(3, 18.193492);
            ageGroups2.Add(4, 12.200275);
            ageGroups2.Add(5, 7.958299);
            ageGroups2.Add(6, 5.089172);
            ageGroups2.Add(7, 3.253833);
            ageGroups2.Add(8, 2.098571);
            ageGroups2.Add(9, 1.359233);
            ageGroups2.Add(10, 0.935798);
            Results.Add(2000, new YearResult(2000, ageGroups2));
        }

        public void RunSimulation(int newYear, IList<ClientState> clients)
        {
            IList<FishingSpot> spots = new List<FishingSpot>();
            int totalShips = 0;
            foreach (var client in clients)
            {
                totalShips += client.FleetSize;
                Debug.LogWarning(client.playerName);
                foreach (var spot in client.fishingspots)
                {
                    spots.Add(spot);
                }
            }

            // Returns distribution of all focus for all groups (spots must contain total spots by all clients)
            IDictionary<int, long> quantity = GetComponent<StockScript>().population.Stock.getMarked(spots);

            long sum = 0;
            foreach (var item in quantity) sum += item.Value;
            
            // Total combined fishing mortality by age
            IDictionary<int, double> CombinedPlayersFishingMortalityRate = new Dictionary<int, double>();
            for (int age = 2; age <= 10; age++)
            {
                double HuntingPressure = 0;
                if (spots.Count > 0 && totalShips > 0)
                {
                    HuntingPressure = ((double)quantity[age] / (double)GetComponent<StockScript>().population.Stock.totalSize) * (double)((double)totalShips / ((double)spots.Count / 2));
                    Debug.LogWarning(HuntingPressure + ", " + totalShips + ", " + spots.Count);
                }

                CombinedPlayersFishingMortalityRate.Add(age, HuntingPressure);
            }
            Debug.LogWarning(newYear + ", Total Pressure: " + sum + " = " + CombinedPlayersFishingMortalityRate[2] + ", " + CombinedPlayersFishingMortalityRate[3] + ", " + CombinedPlayersFishingMortalityRate[4] + ", " + CombinedPlayersFishingMortalityRate[5] + ", " + CombinedPlayersFishingMortalityRate[6] + ", " + CombinedPlayersFishingMortalityRate[7] + ", " + CombinedPlayersFishingMortalityRate[8] + ", " + CombinedPlayersFishingMortalityRate[9] + ", " + CombinedPlayersFishingMortalityRate[10]);

            YearResult result1;
            YearResult result2;
            Results.TryGetValue(newYear - 1, out result1);
            Results.TryGetValue(newYear - 2, out result2);
            Results.Add(newYear, new YearResult(result1, result2, 0.2, CombinedPlayersFishingMortalityRate));
            YearResult latest;
            Results.TryGetValue(newYear, out latest);

            foreach (var client in clients)
            {
                // Calculate 
                IDictionary<int, long> markedByPlayer = GetComponent<StockScript>().population.Stock.getMarked(client.fishingspots);
                IDictionary<int, double> PlayerFishingMortalityRate = new Dictionary<int, double>();
                for (int age = 2; age < 10; age++)
                {
                    double HuntingPressure = 0;
                    if (client.fishingspots.Count > 0 && client.FleetSize > 0)
                    {
                        HuntingPressure = ((double)markedByPlayer[age] / (double)GetComponent<StockScript>().population.Stock.totalAgeGroup[age]) * (double)((double)client.FleetSize / ((double)client.fishingspots.Count / 2));
                        Debug.LogWarning("Is Executed");
                    }
                    PlayerFishingMortalityRate.Add(age, HuntingPressure);
                }
                IDictionary<int, double> PlayerFishedByAge = latest.ReturnFishedByPlayer(PlayerFishingMortalityRate);

                // Calculate Income, Cost, and Funds
                client.Revenue = 0;
                client.Expenses = client.FleetSize * (gameObject.GetComponent<Server>().configuration.MaintenanceFee + gameObject.GetComponent<Server>().configuration.OilPrice);

                client.Fished = new double[10];
                for (int i = 2; i < 10; i++)
                {
                    client.Fished[i - 2] = PlayerFishedByAge[i];

                    // Calculate income
                    if (i == 2)
                        client.Revenue += PlayerFishedByAge[i] * 11.98;
                    if (i == 3)
                        client.Revenue += PlayerFishedByAge[i] * 17.80;
                    if (i == 4)
                        client.Revenue += PlayerFishedByAge[i] * 28.91;
                    if (i == 5)
                        client.Revenue += PlayerFishedByAge[i] * 41.34;
                    if (i == 6)
                        client.Revenue += PlayerFishedByAge[i] * 57.00;
                    if (i == 7)
                        client.Revenue += PlayerFishedByAge[i] * 73.70;
                    if (i == 8)
                        client.Revenue += PlayerFishedByAge[i] * 82.35;
                    if (i == 9)
                        client.Revenue += PlayerFishedByAge[i] * 98.87;
                }
                client.AccumulatedProfit += client.Revenue - client.Expenses;
            }

            // UpdateClients
            gameObject.GetComponent<Server>().readyPlayers = clients;

            // Store Information
            string subPath = gameObject.GetComponent<Server>().StartTime.ToString("yyyy-MM-dd-HH-mm-ss"); // your code goes here
            Debug.LogWarning(System.IO.Directory.GetCurrentDirectory() + "\\Records\\");
            bool exists = System.IO.Directory.Exists(System.IO.Directory.GetCurrentDirectory() + "\\Records\\");
            if (!exists)
                System.IO.Directory.CreateDirectory(System.IO.Directory.GetCurrentDirectory() + "\\Records\\");

            var csv = new System.Text.StringBuilder();

            // Sort network by ID
            List<ClientState> SortedList = clients.OrderBy(o => o.PlayerID).ToList();

            // Write Header
            var newLine = "";
            if (!System.IO.File.Exists(System.IO.Directory.GetCurrentDirectory() + "\\Records\\" + subPath + ".csv"))
            {
                newLine = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13}", "Year", "Age 2", "Age 3", "Age 4", "Age 5", "Age 6", "Age 7", "Age 8", "Age 9", "Age 10", "Total Amount", "Total Weight", "Total Spawn Amount", "Total Spawn Weight");
                newLine += string.Format(",{0},{1},{2},{3},{4},{5},{6},{7},{8}", "Caught Age 2", "Caught Age 3", "Caught Age 4", "Caught Age 5", "Caught Age 6", "Caught Age 7", "Caught Age 8", "Caught Age 9", "Total Caught");

                // Financial header for each player
                foreach (var player in SortedList)
                    newLine += string.Format(",{0},{1},{2},{3}", player.PlayerID + "_" + player.playerName, "Revenue", "Expenses", "Accumulated Profit");

                csv.AppendLine(newLine);
            }
            newLine = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13}", newYear, latest.AgeGroup[2], latest.AgeGroup[3], latest.AgeGroup[4], latest.AgeGroup[5], latest.AgeGroup[6], latest.AgeGroup[7], latest.AgeGroup[8], latest.AgeGroup[9], latest.AgeGroup[10], latest.TotalAmount, latest.TotalWeight, latest.TotalSpawnAmount, latest.TotalSpawnWeight);
            newLine += string.Format(",{0},{1},{2},{3},{4},{5},{6},{7},{8}", latest.AgeGroupFished[2], latest.AgeGroupFished[3], latest.AgeGroupFished[4], latest.AgeGroupFished[5], latest.AgeGroupFished[6], latest.AgeGroupFished[7], latest.AgeGroupFished[8], latest.AgeGroupFished[9], latest.AgeGroupFished[2] + latest.AgeGroupFished[3] + latest.AgeGroupFished[4] + latest.AgeGroupFished[5] + latest.AgeGroupFished[6] + latest.AgeGroupFished[7] + latest.AgeGroupFished[8] + latest.AgeGroupFished[9]);

            // Financial header for each player
            foreach (var player in SortedList)
                newLine += string.Format(",{0},{1},{2},{3}", player.PlayerID + "_" + player.playerName, player.Revenue, player.Expenses, player.AccumulatedProfit);


            csv.AppendLine(newLine);
            System.IO.File.AppendAllText(System.IO.Directory.GetCurrentDirectory() + "\\Records\\" + subPath + ".csv", csv.ToString());
        }
    }
}