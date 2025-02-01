using UnityEngine;
using UnityEngine.Rendering;
using TMPro;

public class TimeShift : MonoBehaviour
{
    public Volume ppv; // this is the post processing volume
    public float tick; // Increasing the tick, increases second rate
    public float seconds;
    public int mins;
    public int hours;
    public int days = 1;
    public bool activateLights; // checks if lights are on
    public GameObject[] lights; // all the lights we want on when its dark

    public TextMeshProUGUI timeDisplay; // Display Time
    public TextMeshProUGUI dayDisplay; // Display Day

    // Start is called before the first frame update
    void Start()
    {
        LoadTimeData();

        ppv = gameObject.GetComponent<Volume>();
        timeDisplay = GameObject.Find("timeDisplay").GetComponent<TextMeshProUGUI>();
        dayDisplay = GameObject.Find("dayDisplay").GetComponent<TextMeshProUGUI>();

        ControlPPVStart();
    }

    // Update is called once per frame
    void FixedUpdate() // we used fixed update, since update is frame dependant.
    {
        CalcTime();

        DisplayTime();
    }

    public void CalcTime() // Used to calculate sec, min and hours
    {
        seconds += Time.fixedDeltaTime * tick; // multiply time between fixed update by tick
        if (seconds >= 60) // 60 sec = 1 min
        {
            seconds = 0;
            mins += 1;
        }

        if (mins >= 60) //60 min = 1 hr
        {
            mins = 0;
            hours += 1;
        }

        if (hours >= 24) //24 hr = 1 day
        {
            hours = 0;
            days += 1;
        }

        ControlPPV(); // changes post processing volume after calculation
    }

    public void ControlPPV() // used to adjust the post processing slider.
    {
//ppv.weight = 0;
        if (hours >= 21 && hours < 22) // dusk at 21:00 / 9pm - until 22:00 / 10pm
        {
            ppv.weight =
                (float)mins /
                60; // since dusk is 1 hr, we just divide the mins by 60 which will slowly increase from 0 - 1
            if (activateLights == false) // if lights havent been turned on
            {
                if (mins > 45) // wait until pretty dark
                {
                    for (int i = 0; i < lights.Length; i++)
                    {
                        lights[i].SetActive(true); // turn them all on
                    }

                    activateLights = true;
                }
            }
        }

        if (hours >= 6 && hours < 7) // Dawn at 6:00 / 6am - until 7:00 / 7am
        {
            ppv.weight = 1 - (float)mins / 60; // we minus 1 because we want it to go from 1 - 0
            if (activateLights) // if lights are on
            {
                if (mins > 45) // wait until pretty bright
                {
                    for (int i = 0; i < lights.Length; i++)
                    {
                        lights[i].SetActive(false); // shut them off
                    }

                    activateLights = false;
                }
            }
        }
    }
    
    public void ControlPPVStart() // used to adjust the post processing slider.
    {
        if (hours >= 21 || hours <= 7)
        {
            ppv.weight = 1.0f;
            for (int i = 0; i < lights.Length; i++)
            {
                lights[i].SetActive(true); // shut them off
            }

            activateLights = true;
        }
        else
        {
            ppv.weight = 0.0f;
            for (int i = 0; i < lights.Length; i++)
            {
                lights[i].SetActive(false); // shut them off
            }

            activateLights = false;
        }
    }

    public void DisplayTime() // Shows time and day in ui 
    {
        timeDisplay.text =
            string.Format("{0:00}:{1:00}", hours,
                mins); // The formatting ensures that there will always be 0's in empty spaces
        dayDisplay.text = "Day: " + days; // display day counter
    }

    private void OnDisable()
    {
        SaveTimeData();
    }

    private void OnApplicationQuit()
    {
        SaveTimeData();
    }

    // Save time and date to PlayerPrefs
    public void SaveTimeData()
    {
        PlayerPrefs.SetInt("Hours", hours);
        PlayerPrefs.SetInt("Minutes", mins);
        PlayerPrefs.SetInt("Days", days);
        PlayerPrefs.Save(); // Make sure to call Save to ensure data is written to disk
        Debug.Log("Time saved!");
    }

    // Load the time data when the scene starts
    public void LoadTimeData()
    {
        if (PlayerPrefs.HasKey("Hours"))
        {
            hours = PlayerPrefs.GetInt("Hours");
            mins = PlayerPrefs.GetInt("Minutes");
            days = PlayerPrefs.GetInt("Days");
            Debug.Log("Time loaded!");
        }
        else
        {
            Debug.Log("No saved time found. Starting fresh.");
        }
    }
}