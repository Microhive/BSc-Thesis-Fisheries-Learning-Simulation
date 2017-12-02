using System;
using System.Collections.Generic;
using Assets.Scripts.Other;
using Assets.Scripts.State;

namespace Assets.Scripts.Population
{
    public class Stock
    {
        public IDictionary<int, long> totalAgeGroup { get; set; }
        public long totalSize = 0;
        Dictionary<Tuple<int, double, double>, int> stock = new Dictionary<Tuple<int, double, double>, int>();

        public Stock(double left, double right, double top, double bottom)
        {
            totalAgeGroup = new Dictionary<int, long>();
            for (int i = 2; i <= 10; i++)
            {
                totalAgeGroup.Add(i, 0);
            }

            for (double x = left; x < right; x += 0.1)
            {
                for (double y = bottom; y < top; y += 0.1)
                {
                    for (int a = 2; a <= 10; a++)
                    {
                        increment(a, x, y, 0);
                    }
                }
            }
        }

        public void increment(int age, double x, double y, int quantity)
        {
            if (stock.ContainsKey(new Tuple<int, double, double>(age, x, y)))
                stock[new Tuple<int, double, double>(age, x, y)] += quantity;
            else
                stock.Add(new Tuple<int, double, double>(age, x, y), quantity);

            totalAgeGroup[age] += quantity;
            totalSize += quantity;
        }

        public int? get(int age, double x, double y)
        {
            if (!stock.ContainsKey(new Tuple<int, double, double>(age, x, y)))
                return null;
            else
                return stock[new Tuple<int, double, double>(age, x, y)];
        }

        public IDictionary<int, long> getMarked(IList<FishingSpot> spots)
        {
            // ToDo(Accounted for, don't recount)
            IDictionary<int, long> returnResult = new Dictionary<int, long>();
            for (int i = 2; i <= 10; i++)
            {
                returnResult.Add(i, 0);
            }

            foreach (var spot in spots)
            {
                double startX = System.Math.Round((spot.X - spot.Width / 2), 1);
                for (double x = startX; x < startX + spot.Width; x += 0.1)
                {
                    //x = 61.5;
                    double startY = System.Math.Round((spot.Y + spot.Width/ 2), 1);
                    for (double y = startY; y < startY + spot.Width; y += 0.1)
                    {
                        x = Math.Round(x, 1);
                        //y = -7.2;
                        for (int age = 2; age < 10; age++)
                        {
                            y = Math.Round(y, 1);
                            int size;
                            if (stock.TryGetValue(new Tuple<int, double, double>(age, x, y), out size))
                            {
                                returnResult[age] += size;
                            }
                        }
                    }
                }
            }

            return returnResult;
        }
    }
}
