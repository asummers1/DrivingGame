//Adapted from Unity Game Development Cookbook, published by O'Reilly Media in March 2019
//My additions involve multiplayer support, a UI (including a countdown at the beginning of each race), music, and
//level transitions after a win.

using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[System.Serializable]
public struct carData
{
    public Transform carTransform;
    public GameObject wrongWayIndicator;
    public Text lapCounter;
    
    [System.NonSerialized]
    public int carDistance;
    [System.NonSerialized]
    public int lapsComplete;
    [System.NonSerialized]
    public Checkpoint nearestCheckpoint;
    [System.NonSerialized]
    public Checkpoint lastSeenCheckpoint;
    public Text movementIndicator;
}
public class LapTracker : MonoBehaviour
{
    
    [System.NonSerialized]
    public float countdownTimer = 3f;


    [SerializeField] carData[] cars;
    [SerializeField] Text txtCountdownTimer;
    [SerializeField] int longestPermittedShortcut = 0;

    [SerializeField] int totalLaps;

    [SerializeField] Text winText;

    [SerializeField] Text visualTotalLaps;
    bool isLevelWon = false;
    RaceTimer timer;
    RaceTimer oldTimer;
    //int lapsComplete = 0;
    Scene sceneToChangeTo;

    Checkpoint lastSeenCheckpoint;

    Checkpoint[] allCheckpoints;
    string gameType;
     Coroutine coro;
    Checkpoint StartCheckpoint
    {
        get
        {
            return FindObjectsOfType<Checkpoint>().Where(c => c.isLapStart).FirstOrDefault();
        }
    }

    private void Start()
    {
        timer = gameObject.GetComponent<RaceTimer>();

        
        if (!File.Exists("GameMode.txt"))
        {
            File.Create("GameMode.txt");
        }
        using (StreamReader streamReader = new StreamReader("GameMode.txt"))
        {
            gameType = streamReader.ReadLine();
        }

        for (int i = 0; i < cars.Length; i++)
        {
            UpdateLapCounter(i);

            cars[i].wrongWayIndicator.SetActive(false);
            cars[i].lastSeenCheckpoint = StartCheckpoint;
        }
        allCheckpoints = FindObjectsOfType<Checkpoint>();

        CreateCircuit();


        
        winText.enabled = false;

        visualTotalLaps.text = totalLaps.ToString();
        SetupElements();

        StopAllCoroutines();

        try //This will throw an error if a developer tries to start a level without going through the main menu. This is fine; there just won't be any audio.
        {
            GameObject.FindGameObjectWithTag("Music").GetComponent<MusicManager>().PlayMusic(SceneManager.GetActiveScene());
        }
        catch
        {

        }
        coro = StartCoroutine(CountdownTimer(3f));
    }
    private void Update()
    {
        //Debug.Log(1.0 / Time.deltaTime); //FPS
        for (int i = 0; i < cars.Length; i++)
        {
            cars[i].nearestCheckpoint = NearestCheckpoint(i);
        }

        if (!isLevelWon)
        {
            for (int i = 0; i < cars.Length; i++)
            {
                if (cars[i].lapsComplete == totalLaps) //Doesn't run when isLevelWon is set to true in this condition
                {
                    isLevelWon = true;

                    Vehicle.Player winningVehicle = cars[i].carTransform.GetComponent<Vehicle>().vehiclesPlayer;
                    winText.enabled = true;
                    if (gameType == "Competition" || gameType == null)
                    {
                        winText.text = ($"Player {i + 1} wins!");
                        SavePlayerData(winningVehicle); //Saves data to a file
                        CompetitionWinner.AddPointTo(i);

                    } else if (gameType == "TimeTrial")
                    {
                        winText.text = "You finished!";
                        timer.StopTimer();

                        RaceTimer.CheckAndSetBetterTime(timer);
                        RaceTimer.SaveTimes();
                    }
                    StartCoroutine(LoadSceneAfterWait(3f));
                    if (gameType == "TimeTrial")
                         GameObject.FindGameObjectWithTag("Music").GetComponent<MusicManager>().StopMusic();
                }
            }
            for (int i = 0; i < cars.Length; i++)
            {
                if (cars[i].nearestCheckpoint == null)
                {
                    Debug.Log("Test");
                    return;
                }
                if (cars[i].nearestCheckpoint.index == cars[i].lastSeenCheckpoint.index)
                { } else if (cars[i].nearestCheckpoint.index > cars[i].lastSeenCheckpoint.index)
                {
                    Debug.Log("test...");
                    var distance = cars[i].nearestCheckpoint.index - cars[i].lastSeenCheckpoint.index;

                    if (distance > longestPermittedShortcut + 1)
                    {
                        cars[i].wrongWayIndicator.SetActive(true);
                    } else
                    {
                        cars[i].lastSeenCheckpoint = cars[i].nearestCheckpoint;

                        cars[i].wrongWayIndicator.SetActive(false);
                    }

                } else if (cars[i].nearestCheckpoint.isLapStart && cars[i].lastSeenCheckpoint.next.isLapStart)
                {
                    Debug.Log("Test!");
                    cars[i].lastSeenCheckpoint = cars[i].nearestCheckpoint;

                    cars[i].lapsComplete += 1;
                    UpdateLapCounter(i);
                } else
                {
                    //Checkpoint is lower than the last one, player is going the wrong way
                    cars[i].wrongWayIndicator.SetActive(true);
                }
            }
        }

    }

    IEnumerator CountdownTimer(float timeRemaining = 3)
    {

        cars[0].carTransform.GetComponent<Vehicle>().enabled = false;
        cars[1].carTransform.GetComponent<Vehicle>().enabled = false;

        while (timeRemaining > 0)
        {

            timeRemaining--;
            yield return new WaitForSeconds(1f);
            txtCountdownTimer.text = timeRemaining.ToString();


        }
        cars[0].carTransform.GetComponent<Vehicle>().enabled = true;
        cars[1].carTransform.GetComponent<Vehicle>().enabled = true;
        txtCountdownTimer.text = "";
        timer.StartTimer();
    }
    Checkpoint NearestCheckpoint(int carIndex)
    {
        if (allCheckpoints == null)
        {
            return null;
        }
        Checkpoint nearestSoFar = null;
        float nearestDistanceSoFar = float.PositiveInfinity;

        for (int c = 0; c < allCheckpoints.Length; c++)
        {
            var checkpoint = allCheckpoints[c];
            var distance = (cars[carIndex].carTransform.position - checkpoint.transform.position).sqrMagnitude;

            if (distance < nearestDistanceSoFar)
            {
                nearestSoFar = checkpoint;
                nearestDistanceSoFar = distance;
            }
        }
        return nearestSoFar;
    }
    void CreateCircuit()
    {
        var index = 0;

        var currentCheckpoint = StartCheckpoint;

        do
        {
            currentCheckpoint.index = index;
            index += 1;

            currentCheckpoint = currentCheckpoint.next;

            if (currentCheckpoint == null)
            {
                Debug.LogError("The circuit is not closed");
                return;
            }
        } while (currentCheckpoint.isLapStart == false);
    }
    void UpdateLapCounter(int carIndex)
    {
        cars[carIndex].lapCounter.text = string.Format($"Lap {cars[carIndex].lapsComplete + 1}");
    }
    private void OnDrawGizmos()
    {
        var nearest = NearestCheckpoint(0);

        if (cars[0].carTransform != null && nearest != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(cars[0].carTransform.position, nearest.transform.position);
        }
    }
    void SavePlayerData(Vehicle.Player winningPlayer)
    {
        if (!File.Exists("PlayerData.txt"))
        {
            File.Create("PlayerData.txt");
        }
        using (StreamWriter strm = new StreamWriter("PlayerData.txt"))
        {
            strm.WriteLine(winningPlayer);
            Debug.Log(winningPlayer);
        }
    }
   
    IEnumerator LoadSceneAfterWait(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        if (gameType == "Competition")
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        } else
        {
            SceneManager.LoadScene(0); //Go to title screen
        }
        
    }
    private void SetupElements()
    {
        if (gameType == "Competition")
        {
            timer.timerText.enabled = false;
            timer.oldTime.enabled = false;
            //GameObject.FindGameObjectWithTag("BestTimeLabel").GetComponent<Text>().text = "";
        }
        else if (gameType == "TimeTrial")
        {
            for (int i = 1; i < cars.Length; i++)
            {
                cars[i].carTransform.gameObject.SetActive(false); //Disable all but player 1
                cars[i].wrongWayIndicator.SetActive(false);
                cars[i].lapCounter.text = "";
                cars[i].movementIndicator.text = "";
                

            }
            if (!File.Exists("BestTime.txt"))
            {
                File.Create("BestTime.txt");
                oldTimer = null;
            } 
            else
            {
                oldTimer = RaceTimer.GetOldBestTime();
            }

            MenuManager.startFromTimeTrial = true;
        }
    }
}
