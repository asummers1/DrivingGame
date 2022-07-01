using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameModeSetter : MonoBehaviour
{
   public enum GameType
    {
        Competition, 
        TimeTrial
    }
    // Start is called before the first frame update
    void Start()
    {
      if (!File.Exists("GameMode.txt"))
        {  
            File.Create("GameMode.txt");
        }
    }
    public void chooseGameType(string chosenGameType)
    {
       using (StreamWriter strm = new StreamWriter("GameMode.txt", false))
        {
            strm.WriteLine(chosenGameType.ToString());
        }
    }
}
