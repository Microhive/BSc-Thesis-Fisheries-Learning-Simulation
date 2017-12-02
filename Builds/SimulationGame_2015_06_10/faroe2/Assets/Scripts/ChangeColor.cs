using UnityEngine;
using System.Collections;

public class ChangeColor : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Renderer obj = GetComponent<Renderer> ();
		if (obj != null) {
			obj.material.color = new Color(0.5f,1,1); //C#
		}
	}

}
