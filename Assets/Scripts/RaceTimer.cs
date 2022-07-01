using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RaceTimer : MonoBehaviour
{
    public Text timerText;
    public Text oldTime;
    private static RaceTimer firstTime;
    private static RaceTimer secondTime;
    private static RaceTimer thirdTime;
    private static RaceTimer fourthTime;
    private static RaceTimer fifthTime;
    private float seconds;
    private float minutes;
    private float hours;
    private bool isTimerOn = false;
    private static bool quitting = false;
    // Start is called before the first frame update
    //static Dictionary<int, RaceTimer> timers = new Dictionary<int, RaceTimer>(); //Handles all timers
    private void Start()
    {
        using (StreamReader strm = new StreamReader("BestTime.txt"))
        {
            while (!strm.EndOfStream)
            {

                string val = strm.ReadLine();
               
                if (val.Contains("FirstTime"))
                {
                    Debug.Log("TestRace2");
                    string[] first = val.Split();
                    firstTime = ConvertToRaceTimer(first[1]);
                }
                if (val.Contains("SecondTime"))
                {
                    string[] second = val.Split();
                    secondTime = ConvertToRaceTimer(second[1]);
                }
                if (val.Contains("ThirdTime"))
                {
                    string[] third = val.Split();
                    thirdTime = ConvertToRaceTimer(third[1]);
                }
                if (val.Contains("FourthTime"))
                {
                    string[] fourth = val.Split();
                    fourthTime = ConvertToRaceTimer(fourth[1]);
                }
                if (val.Contains("FifthTime"))
                {
                    string[] fifth = val.Split();
                    fifthTime = ConvertToRaceTimer(fifth[1]);
                }
            }
        }

        switch (SceneManager.GetActiveScene().buildIndex)
        {
            case 1:
                DisplayTime(firstTime, oldTime, "Old");
                break;
            case 2:
                DisplayTime(secondTime, oldTime, "Old");
                break;
            case 3:
                DisplayTime(thirdTime, oldTime, "Old");
                break;
            case 4:
                DisplayTime(fourthTime, oldTime, "Old");
                break;
            case 5:
                DisplayTime(fifthTime, oldTime, "Old");
                break;

        }

    }
    private void Update()
    {
        if (isTimerOn)
        {
            seconds += Time.deltaTime;

            if (seconds >= 60)
            {
                minutes++;
                seconds = 0;
            } else if (minutes >= 60)
            {
                hours++;
                minutes = 0;
            }
            DisplayTime(this, timerText, "New");
        }
    }
    
    public static bool operator<(RaceTimer one, RaceTimer two)
    {
        //Standard null checking does not work in this scenario, because Unity overloads the equality
        //operator for objects derived from MonoBehaviour. So, objects that aren't actually null return
        //null in comparison operations, and vice versa.
        //Instead, we need to use this:
        //https://forum.unity.com/threads/fun-with-null.148090/
        if (!EqualityComparer<RaceTimer>.Default.Equals(two, default(RaceTimer)))
        {
            int firstTimeSeconds = ((Convert.ToInt32(one.hours) * 3600) + (Convert.ToInt32(one.minutes) * 60) + Convert.ToInt32(one.seconds));
            int secondTimeSeconds = ((Convert.ToInt32(two.hours) * 3600) + (Convert.ToInt32(two.minutes) * 60) + Convert.ToInt32(two.seconds));
            return (firstTimeSeconds < secondTimeSeconds);
        }
        return true;
    }
    public static bool operator>(RaceTimer one, RaceTimer two)
    {
        if (!EqualityComparer<RaceTimer>.Default.Equals(two, default(RaceTimer)))
        {
            int firstTimeSeconds = ((Convert.ToInt32(one.hours) * 3600) + (Convert.ToInt32(one.minutes) * 60) + Convert.ToInt32(one.seconds));
            int secondTimeSeconds = ((Convert.ToInt32(two.hours) * 3600) + (Convert.ToInt32(two.minutes) * 60) + Convert.ToInt32(two.seconds));
            return (firstTimeSeconds > secondTimeSeconds);
        }

        return false;
    }
    public void StartTimer()
    {
        isTimerOn = true;
    }
    public void StopTimer()
    {
        isTimerOn = false;
    }
    public static void CheckAndSetBetterTime(RaceTimer newTime)
    { 
            switch (SceneManager.GetActiveScene().buildIndex)
            {
                case 1:
                    if (newTime < firstTime)
                    {
                        firstTime = newTime;
                    }
                    break;
                case 2:
                    if (newTime < secondTime)
                    {
                        secondTime = newTime;
                    }
                    break;
                case 3:
                    if (newTime < thirdTime)
                    {
                        thirdTime = newTime;
                    }
                    break;
                case 4:
                    if (newTime < fourthTime)
                    {
                        fourthTime = newTime;
                    }
                    break;
                case 5:
                    if (newTime < fifthTime)
                    {
                        fifthTime = newTime;
                    }
                    break;
            
        }
        
    }
    public static void SaveTimes()
    {
        if (!quitting) //Player completed level, not quitted out
        {
            File.WriteAllText("BestTime.txt", string.Empty); //Clears out text for new input
            using (StreamWriter strm = new StreamWriter("BestTime.txt"))
            {
                //Standard null checking does not work in this scenario, because Unity overloads the equality
                //operator for objects derived from MonoBehaviour. So, objects that aren't actually null return
                //null in comparison operations, and vice versa.
                //Instead, we need to use the EqualityComparer to check if an existing object isn't at its initial state:
                //https://forum.unity.com/threads/fun-with-null.148090/
                if (!EqualityComparer<RaceTimer>.Default.Equals(firstTime, default(RaceTimer)))
                {
                    strm.WriteLine("FirstTime: " + firstTime.ToString());
                }
                if (!EqualityComparer<RaceTimer>.Default.Equals(secondTime, default(RaceTimer)))
                {
                    strm.WriteLine("SecondTime: " + secondTime.ToString());
                }
                if (!EqualityComparer<RaceTimer>.Default.Equals(thirdTime, default(RaceTimer)))
                {
                    strm.WriteLine("ThirdTime: " + thirdTime.ToString());
                }
                if (!EqualityComparer<RaceTimer>.Default.Equals(fourthTime, default(RaceTimer)))
                {
                    strm.WriteLine("FourthTime: " + fourthTime.ToString());
                }
                if (!EqualityComparer<RaceTimer>.Default.Equals(fifthTime, default(RaceTimer)))
                {
                    strm.WriteLine("FifthTime: " + fifthTime.ToString());
                }
            }
            quitting = false; //Reset value
        }
    }
    private void DisplayTime(RaceTimer timeToDisplay, Text textToDisplayIn, string timerType)
    {
        /*
        bool compare;
        if (timeToDisplay.seconds != 0 || timeToDisplay.minutes != 0 || timeToDisplay.hours != 0)
        {
            if (timerType == "New")
            {
                compare = (timeToDisplay != null);
            } else
            {
                compare = (timeToDisplay == null);
            }
            if (compare)
            {
                textToDisplayIn.text = $"{Convert.ToInt32(timeToDisplay.hours)}:{timeToDisplay.minutes}:{(int)timeToDisplay.seconds}";
            }
        }
        
        else
        {
            textToDisplayIn.text = "First time";
        }
        */
        if (timerType == "New")
        {
            textToDisplayIn.text = $"Current time: {Convert.ToInt32(timeToDisplay.hours)}:{timeToDisplay.minutes}:{(int)timeToDisplay.seconds}";
        }
        else if (timerType == "Old")
        {
            textToDisplayIn.text = $"Best time: {Convert.ToInt32(timeToDisplay.hours)}:{timeToDisplay.minutes}:{(int)timeToDisplay.seconds}";
        }
    }
    
    public static RaceTimer GetOldBestTime()
    {
        switch (SceneManager.GetActiveScene().buildIndex)
        {
            case 1:
                return firstTime;
            case 2:
                return secondTime;
            case 3:
                return thirdTime;
            case 4:
                return fourthTime;
            case 5:
                return fifthTime;
            default:
                return null;
        }
    }
    private void OnApplicationQuit()
    {
        quitting = true;
    }

    
    /// <summary>
    /// Converts time to its string representation, formatted as seconds
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        int num = (int)(hours * 3600) + (int)(minutes * 60) + (int)(seconds);
        return num.ToString();
    }
    public RaceTimer ConvertToRaceTimer(string time)
    {

        int totalSeconds = System.Convert.ToInt32(time);

        RaceTimer raceTime = new RaceTimer();
        //Divide by 60 to get the number of minutes in seconds; the "remainder" is what is left (our seconds)
        raceTime.minutes = totalSeconds / 60;
        raceTime.seconds = totalSeconds % 60;

        //Divide by 60 to get the number of hours in minutes; the "remainder" is what is left (our minutes)
        raceTime.hours = raceTime.minutes / 60;
        raceTime.minutes = raceTime.minutes % 60;

        return raceTime;
    }

}
