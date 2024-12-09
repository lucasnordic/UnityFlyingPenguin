using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeUI : MonoBehaviour
{
    public float duration = 1.5f;  // How long the shake lasts
    public float magnitude = 5f; // How strong the shake is

    private RectTransform rectTransform;
    private Vector2 originalPosition;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        originalPosition = rectTransform.anchoredPosition;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartShake();
        }
    }

    public void StartShake()
    {
        StopAllCoroutines(); // Stops existing shakes
        StartCoroutine(Shake());
    }

    private System.Collections.IEnumerator Shake()
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float offsetX = Random.Range(-1f, 1f) * magnitude;
            float offsetY = Random.Range(-1f, 1f) * magnitude;

            rectTransform.anchoredPosition = originalPosition + new Vector2(offsetX, offsetY);

            yield return null; // Wait for the next frame
        }

        rectTransform.anchoredPosition = originalPosition;
    }
}