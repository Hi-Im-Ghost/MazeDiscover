using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SuccessCounter : MonoBehaviour
{
    [SerializeField] TMP_Text wrongText;
    [SerializeField] TMP_Text timeText;
    [SerializeField] TMP_Text goodText;
    private int successCount = 0;
    private int failureCount = 0;
    private int timeoutCount = 0;

    private void Start()
    {
        // Sprawdzenie czy sa komponenty 
        if (goodText == null || wrongText == null || timeText == null)
        {
            Debug.LogError("One or more TMP_Text components not assigned!");
        }
        else
        {
            UpdateCountersText();
        }
    }

    // Metoda do aktualizacji tekstu
    private void UpdateCountersText()
    {
        goodText.text = successCount.ToString();
        wrongText.text = failureCount.ToString();
        timeText.text = timeoutCount.ToString();
    }

    // Metoda do inkrementacji licznika sukcesów i aktualizacji tekstu
    public void IncrementSuccessCount()
    {
        successCount++;
        UpdateCountersText();
    }

    // Metoda do inkrementacji licznika b³êdów i aktualizacji tekstu
    public void IncrementFailureCount()
    {
        failureCount++;
        UpdateCountersText();
    }

    // Metoda do inkrementacji licznika przekroczenia czasu i aktualizacji tekstu
    public void IncrementTimeoutCount()
    {
        timeoutCount++;
        UpdateCountersText();
    }
}

