using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Assets.Scripts.Simulation
{
    [System.Serializable]
    public class YearResult
    {
        public int Year { get; set; }
        public IDictionary<int, double> AgeGroup { get; set; }
        public IDictionary<int, double> AgeGroupFished { get; set; }
        public double TotalAmount { get; set; }
        public double TotalWeight { get; set; }
        public double TotalSpawnAmount { get; set; }
        public double TotalSpawnWeight { get; set; }
        public double NaturalMortalityRate { get; set; }
        public IDictionary<int, double> FishingMortalityRateAtAge { get; set; }

        public YearResult(int year, IDictionary<int, double> ageGroup, double naturalMortalityRate = 0.2)
        {
            Year = year;
            AgeGroup = ageGroup;
            NaturalMortalityRate = naturalMortalityRate;
            FishingMortalityRateAtAge = new Dictionary<int, double>();
            for (int age = 2; age <= 10; age++)
            {
                FishingMortalityRateAtAge.Add(age, 0);
            }
            CalculateBasedOnAgeGroups();
        }

        public YearResult(YearResult result, YearResult result2, double naturalMortalityRate = 0.2, IDictionary<int, double> fishingMortalityRate = null)
        {
            Year = result.Year + 1;
            AgeGroup = new Dictionary<int, double>();
            NaturalMortalityRate = naturalMortalityRate;

            if (fishingMortalityRate != null)
            {
                FishingMortalityRateAtAge = fishingMortalityRate;
            }
            else
            {
                for (int age = 2; age <= 10; age++)
                {
                    FishingMortalityRateAtAge[age] = 0;
                }
            }

            foreach (var group in result.AgeGroup)
            {
                // Calculate recruitment
                if (group.Key == 2)
                {
                    // =0.25*(N2)/(1+EXP(9.185*((N2)/129-1)))
                    double calc = 0.25 * (result2.TotalSpawnWeight) / (1 + System.Math.Pow(System.Math.E, 9.185 * ((result2.TotalSpawnWeight) /129-1)));
                    AgeGroup.Add(2, calc);
                    continue;
                }

                // Aging of stock
                //= B3 * EXP(-($O4 +$P4))
                AgeGroup.Add(group.Key, result.AgeGroup[group.Key - 1] * System.Math.Pow(System.Math.E, -(NaturalMortalityRate + FishingMortalityRateAtAge[group.Key])));
            }

            CalculateBasedOnAgeGroups();
            CalculateFishingMortalityRate(result);
        }

        private void CalculateBasedOnAgeGroups()
        {
            TotalAmount = AgeGroup[2] + AgeGroup[3] + AgeGroup[4] + AgeGroup[5] + AgeGroup[6] + AgeGroup[7] + AgeGroup[8] + AgeGroup[9] + AgeGroup[10];
            TotalWeight = AgeGroup[2] * 1 + AgeGroup[3] * 1.5 + AgeGroup[4] * 2 + AgeGroup[5] * 3.5 + AgeGroup[6] * 4 + AgeGroup[7] * 5 + AgeGroup[8] * 6 + AgeGroup[9] * 8 + AgeGroup[10] * 10;
            TotalSpawnAmount = AgeGroup[3] * 0.5 + AgeGroup[4] * 0.8 + AgeGroup[5] + AgeGroup[6] + AgeGroup[7] + AgeGroup[8] + AgeGroup[9] + AgeGroup[10];
            TotalSpawnWeight = AgeGroup[3] * 0.5 * 1.5 + AgeGroup[4] * 0.8 * 0.8 + AgeGroup[5] * 3.5 + AgeGroup[6] * 4 + AgeGroup[7] * 5 + AgeGroup[8] * 6 + AgeGroup[9] * 8 + AgeGroup[10] * 10;
        }

        private void CalculateFishingMortalityRate(YearResult PreviousYear)
        {
            AgeGroupFished = new Dictionary<int, double>();
            for (int age = 2; age < 10; age++)
            {
                AgeGroupFished[age] = (PreviousYear.AgeGroup[age] - AgeGroup[age + 1]) * (FishingMortalityRateAtAge[age] / (FishingMortalityRateAtAge[age] + NaturalMortalityRate));
            }
        }

        public IDictionary<int, double> ReturnFishedByPlayer(IDictionary<int, double> fishingMortalityRateByPlayer)
        {
            IDictionary<int, double> ListFishedByPlayer = new Dictionary<int, double>();
            foreach (var age in FishingMortalityRateAtAge)
            {
                double temp = 0;
                if (FishingMortalityRateAtAge[age.Key] > 0)
                {
                    temp = (fishingMortalityRateByPlayer[age.Key] / FishingMortalityRateAtAge[age.Key]) * (double)AgeGroupFished[age.Key];
                }
                ListFishedByPlayer.Add(age.Key, temp);
            }

            return ListFishedByPlayer;
        }
    }
}
