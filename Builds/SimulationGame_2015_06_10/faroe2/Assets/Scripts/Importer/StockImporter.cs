using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Assets.Scripts.Population;

namespace Assets.Scripts.Importer
{
    public static class StockImporter
    {
        public static void Import(string filename, out Stock stock)
        {
            stock = new Stock(-11, -3, 63, 60);

            var reader = new StreamReader(File.OpenRead(@"Assets\Data\" + filename));

            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var values = line.Split(',');
                stock.increment(Int32.Parse(values[1]), Double.Parse(values[2]), Double.Parse(values[3]), Int32.Parse(values[4]));
            }
        }
    }
}
