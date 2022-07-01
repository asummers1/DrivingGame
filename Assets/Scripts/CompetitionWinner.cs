using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CompetitionWinner : MonoBehaviour
{
    static int[] playerScores = new int[2];
    public Text winningText;
    // Start is called before the first frame update
    void Start()
    {
        
        int topScore = 0;
        int carIndex = 0;
        for (int i = 0; i < playerScores.Length; i++)
        {
            if (playerScores[i] > topScore)
            {
                topScore = playerScores[i];
                carIndex = i;
            }
        }
        winningText.text = $"Player {carIndex + 1} wins! \n Won {topScore} levels";
    }
    static public void AddPointTo(int i)
    {
        playerScores[i]++;
    }
}
