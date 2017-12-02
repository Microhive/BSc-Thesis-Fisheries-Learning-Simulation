using Assets.Scripts.Importer;

namespace Assets.Scripts.Population
{
    public class Population
    {
        private Stock _stock;

        public Stock Stock
        {
            get { return this._stock; }
        }

        public Population()
        {
            // Load stock
            StockImporter.Import("analysed.csv", out _stock);
        }
    }
}
