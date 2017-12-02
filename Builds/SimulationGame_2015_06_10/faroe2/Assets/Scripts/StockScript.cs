using UnityEngine;
using Assets.Scripts.Population;

public class StockScript : MonoBehaviour {

    public Population population = new Population();

    // Use this for initialization
    void Start()
    {
        //Debug.LogError(population.Stock.get(2, 61.5, - 7.2));
    }

    public Vector2 convertSpotToCoordinate(GameObject fishingSpot)
    {
        double x = -System.Math.Abs(-200 - fishingSpot.transform.position.x) / 1500 * 5.2 - 4.1;
        double z = System.Math.Abs(1400 - fishingSpot.transform.position.z) / 1500 * 2.1 + 60.6;

        x = System.Math.Round(x, 1);
        z = System.Math.Round(z, 1);

        //Debug.LogWarning(x + ", " + z);

        return new Vector2((float)x, (float)z);
    }
}