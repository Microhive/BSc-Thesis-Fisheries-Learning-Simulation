using UnityEngine;
using System.Collections;

public class PlayerUI : MonoBehaviour {

	int numberOfShips = 0;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnGUI()
	{
		// Left


		// Right
		GUI.Label (new Rect (Screen.width - 200, 50, 200, 25), "Veiðutryst: " + numberOfShips);
		if (GUI.Button (new Rect (Screen.width - 110, 50, 25, 25), "+1")) {
			++numberOfShips;
		}
		if (GUI.Button (new Rect (Screen.width - 75, 50, 25, 25), "-1")) {
			if (numberOfShips > 0)
				--numberOfShips;
		}
	}
}
