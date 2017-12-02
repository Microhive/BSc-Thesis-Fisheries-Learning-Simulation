using UnityEngine;
using System.Collections;

public class PlayerControls : MonoBehaviour {

	public Transform target;

	// Use this for initialization
	void Start ()
    {
	    
	}
	
	// Update is called once per frame
	void Update ()
    {
        // Instantiate FishingSpot
        if (Input.GetMouseButtonDown(0) && Input.GetKey(KeyCode.LeftAlt))
        {
			if (target != null)
			{
                Transform clone = Instantiate(target, GetMousePointOnOcean(), target.rotation) as Transform;
                //Transform clone = Network.Instantiate(target, GetMousePointOnOcean(), target.rotation, 0) as Transform;
                clone.position = new Vector3(clone.position.x, 218.837f, clone.position.z);
                //NetworkView view = clone.GetComponent<NetworkView>();
            }
        }
	}

    Vector3 GetMousePointOnOcean()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            return hit.point;
        }

        return new Vector3();
    }
}
