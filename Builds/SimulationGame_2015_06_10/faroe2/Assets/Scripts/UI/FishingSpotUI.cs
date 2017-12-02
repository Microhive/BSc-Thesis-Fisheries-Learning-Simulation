using UnityEngine;
using System;
using System.Globalization;

namespace Assets.Scripts.State
{
    public class FishingSpotUI : MonoBehaviour
    {
        public bool toggleShow = false;
        public bool display = true;
        Transform target;
        Camera mainCamera;
        Vector3 screenPosition;
        public Vector3 offset;

        public DateTime? periodFrom;
        public DateTime? periodTo;
        public string stringDatePeriodFrom = "01-02";
        public string stringDatePeriodTo = "30-04";
        string pattern = "dd-MM";

        // Use this for initialization
        void Start()
        {
            mainCamera = Camera.main;
            target = GetComponent<Transform>();
        }

        void Update()
        {
            screenPosition = mainCamera.WorldToScreenPoint(target.position + offset);
            screenPosition.y = Screen.height - screenPosition.y;
            display = GetComponent<Renderer>().isVisible;

            //Validate input
            DateTime parsedDate;
            if (DateTime.TryParseExact(stringDatePeriodFrom, pattern, null, DateTimeStyles.None, out parsedDate))
                periodFrom = parsedDate;
            else
                periodFrom = null;

            if (DateTime.TryParseExact(stringDatePeriodTo, pattern, null, DateTimeStyles.None, out parsedDate))
                periodTo = parsedDate;
            else
                periodTo = null;

            if (periodFrom.HasValue && periodTo.HasValue && (periodFrom.Value > periodTo.Value))
            {
                periodFrom = null;
                periodTo = null;
            }
        }

        void OnGUI()
        {
            if (display)
            {
                if (toggleShow)
                {
                    toggleShow = GUI.Toggle(new Rect(screenPosition.x, screenPosition.y, 40, 20), toggleShow, "Øki");
                    GUI.color = Color.red;
                    if (GUI.Button(new Rect(screenPosition.x + 80, screenPosition.y, 20, 20), "X"))
                    {
                        Destroy(gameObject, 0.0f);
                    }
                    GUI.color = Color.white;
                    //GUI.Label(new Rect(screenPosition.x, screenPosition.y + 20, 50, 20), "Frá");
                    //GUI.Label(new Rect(screenPosition.x, screenPosition.y + 40, 200, 20), "Til");
                    GUI.color = periodFrom.HasValue ? Color.white : Color.red;
                    stringDatePeriodFrom = "01-02";//GUI.TextField(new Rect(screenPosition.x + 30, screenPosition.y + 20, 70, 20), stringDatePeriodFrom, 5);
                    GUI.color = periodTo.HasValue ? Color.white : Color.red;
                    stringDatePeriodTo = "01-02";//GUI.TextField(new Rect(screenPosition.x + 30, screenPosition.y + 40, 70, 20), stringDatePeriodTo, 5);
                }
                else
                {
                    GUI.color = periodFrom.HasValue && periodTo.HasValue ? Color.white : Color.red;
                    toggleShow = GUI.Toggle(new Rect(screenPosition.x, screenPosition.y, 60, 20), toggleShow, "+");
                }
            }
        }
    }
}