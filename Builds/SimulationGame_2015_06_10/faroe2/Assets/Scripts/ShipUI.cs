using UnityEngine;
using System.Collections;

public class ShipUI : MonoBehaviour {

    public bool toggleHide = false;
    public bool display = false;
    Transform target;
    Camera mainCamera;
    Vector3 screenPosition;
    public Vector3 offset;

    // Zoom in
    //bool lerpZoom = false;

	// Use this for initialization
	void Start () {
        mainCamera = Camera.main;
        target = GetComponent<Transform>();
	}
	
	// Update is called once per frame
	void Update () {
        screenPosition = mainCamera.WorldToScreenPoint(target.position + offset);
        screenPosition.y = Screen.height - screenPosition.y;
        display = GetComponentInChildren<Renderer>().isVisible;
	}

    void FixedUpdate()
    {
        //valToBeLerped = Mathf.Lerp(0, 3, tParam);
    }

    void OnGUI()
    {
        if (display)
        {
            if (toggleHide)
                toggleHide = GUI.Toggle(new Rect(screenPosition.x, screenPosition.y, 60, 20), toggleHide, "Kollurin");
            else
                toggleHide = GUI.Toggle(new Rect(screenPosition.x, screenPosition.y, 60, 20), toggleHide, "Kollurin");

            if (toggleHide)
            {
                if (GUI.Button(new Rect(screenPosition.x, screenPosition.y + 20, 50, 20), "Zoom"))
                {
                    //lerpZoom = true;
                }
            }
        }
    }
}
