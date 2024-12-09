using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ScoreData
{
    public string playerName;
    public int score;

    public ScoreData(string playerName, int score)
    {
        this.playerName = playerName;
        this.score = score;
    }
}
