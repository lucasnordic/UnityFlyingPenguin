using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CountdownManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Text countdownText;
    [SerializeField] private float countdownDelay = 1f;

    private void Start()
    {
        if (countdownText != null) countdownText.gameObject.SetActive(false);
    }

    public IEnumerator StartCountdown(System.Action onCountdownComplete)
    {
        if (countdownText == null) yield break;

        countdownText.gameObject.SetActive(true);

        // 3, 2, 1, GO!
        countdownText.text = "3";
        yield return new WaitForSeconds(countdownDelay);
        countdownText.text = "2";
        yield return new WaitForSeconds(countdownDelay);
        countdownText.text = "1";
        yield return new WaitForSeconds(countdownDelay);
        countdownText.text = "GO!";
        yield return new WaitForSeconds(countdownDelay);

        countdownText.gameObject.SetActive(false);
        onCountdownComplete?.Invoke();
    }
}