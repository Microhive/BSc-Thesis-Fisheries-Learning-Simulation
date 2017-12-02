using UnityEngine;
using System.Collections;

public class TargetMove : MonoBehaviour {

	public float speed = 0.1f;
	private float startTime;
	private float journeyLength;

	Vector3? oldPosition = null;
	Vector3? newPosition = null;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

		if (oldPosition.HasValue && newPosition.HasValue)
		{
			float distCovered = (Time.time - startTime) * speed;
			float fracJourney = distCovered / journeyLength;
			transform.position = Vector3.Lerp(oldPosition.Value, newPosition.Value, fracJourney);
			transform.LookAt(newPosition.Value);
		}

        if (Input.GetMouseButtonDown(0) && Input.GetKey(KeyCode.LeftControl))
		{
			if (oldPosition.HasValue && newPosition.HasValue)
			{
				oldPosition = null;
				newPosition = null;
			}

			if (oldPosition.HasValue)
			{
				startTime = Time.time;
				newPosition = GetMousePointOnOcean();
				newPosition = new Vector3(newPosition.Value.x, transform.position.y, newPosition.Value.z);
				journeyLength = Vector3.Distance(oldPosition.Value, newPosition.Value);
				Debug.Log(Input.mousePosition);
			}
			else
			{
				oldPosition = GetMousePointOnOcean();
				oldPosition = new Vector3(oldPosition.Value.x, transform.position.y, oldPosition.Value.z);
				newPosition = null;
				Debug.Log(Input.mousePosition);
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
