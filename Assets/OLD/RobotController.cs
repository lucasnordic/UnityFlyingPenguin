using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RobotController : MonoBehaviour
{
    public TMP_Text scoreText;
    public TMP_Text lifeText;
    private int score;
    private int life;
    private StateMachine stateMachine;
    [SerializeField] private AudioSource coinAudio;

    private void Start()
    {
        score = 0;
        life = 3;
        setText(scoreText, "Score: ", score);
        setText(lifeText, "Life: ", life);
        coinAudio = GetComponent<AudioSource>();

        stateMachine = GetComponentInChildren<StateMachine>();
        if (stateMachine == null)
        {
            Debug.LogError("State handler not found in children of " + gameObject.name);
        }
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Coin"))
        {
            collision.gameObject.SetActive(false);
            score++;
            setText(scoreText, "Score: ", score);
            coinAudio.Play();
        }

        if (collision.CompareTag("Fire"))
        {
            life--;
            stateMachine.ResetState();
            setText(lifeText, "Life: ", life);
        }

        if (collision.CompareTag("Door"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

    private void setText(TMP_Text t, string text, int val)
    {
        t.text = text + val;
    }
}
