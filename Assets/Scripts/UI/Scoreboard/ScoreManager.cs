using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using System;

public class ScoreManager : MonoBehaviour
{
    private const string HIGH_SCORE_KEY = "HighScore";
    private const int MAX_SCORES = 5;
    private List<ScoreData> highScores;

    private void Awake()
    {
        LoadScores();
    }

    public void AddScore(string playerName, int score)
    {
        if (highScores == null)
            highScores = new List<ScoreData>();

        highScores.Add(new ScoreData(playerName, score));
        highScores = highScores.OrderByDescending(x => x.score).Take(MAX_SCORES).ToList();
        SaveScores();
    }

    public List<ScoreData> GetHighScores()
    {
        return highScores ?? new List<ScoreData>();
    }

    public void SaveScores()
    {
        var wrapper = new ScoresWrapper(highScores);
        string json = JsonUtility.ToJson(wrapper);
        Debug.Log($"Saving JSON: {json}");
        PlayerPrefs.SetString(HIGH_SCORE_KEY, json);
        PlayerPrefs.Save();
    }

    private void LoadScores()
    {
        if (PlayerPrefs.HasKey(HIGH_SCORE_KEY))
        {
            string json = PlayerPrefs.GetString(HIGH_SCORE_KEY);
            var wrapper = JsonUtility.FromJson<ScoresWrapper>(json);
            if (wrapper != null && wrapper.scores != null)
            {
                highScores = wrapper.scores.ToList();
            }
        } else
        {
            highScores = new List<ScoreData>();
        }
    }

    [System.Serializable]
    private class ScoresWrapper
    {
        public ScoreData[] scores;

        public ScoresWrapper(List<ScoreData> scoresList)
        {
            scores = scoresList.ToArray();
        }
    }
}
