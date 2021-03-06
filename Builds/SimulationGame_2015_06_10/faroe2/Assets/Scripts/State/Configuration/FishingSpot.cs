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
using UnityEngine;

namespace Assets.Scripts.State
{
    [Serializable]
	public class FishingSpot
    {
		public double X = 0;
		public double Y = 0;
        public double Width = 4.1;
        public DateTime FromDate = new DateTime ();
		public DateTime ToDate = new DateTime ();

		public FishingSpot ()
		{

		}

		public FishingSpot (double x, double y, double width, DateTime fromDate, DateTime toDate)
		{
			X = x;
			Y = y;
            Width = width;
            FromDate = fromDate;
			ToDate = toDate;
		}

        public void convertSpotToCoordinate(GameObject fishingSpot)
        {
            double x = -System.Math.Abs(-200 - fishingSpot.transform.position.x) / 1500 * 5.2 - 4.1;
            double z = System.Math.Abs(1400 - fishingSpot.transform.position.z) / 1500 * 2.1 + 60.6;

            X = System.Math.Round(z, 1);
            Y = System.Math.Round(x, 1);
        }
    }
}

