using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Widget : MonoBehaviour
{
    public delegate void IntValueUpdatedEventHandler(int value1, int value2);
    public static event IntValueUpdatedEventHandler OnIntValueUpdated;

    [SerializeField] TMP_Text wrongText;
    [SerializeField] TMP_Text timeText;
    [SerializeField] TMP_Text goodText;
    [SerializeField] Button myButton;

    private int successCount = 0;
    private int failureCount = 0;
    private int timeoutCount = 0;

    int ValX;
    int ValY;

    private void Start()
    {
        // Dodajemy funkcjê, która zostanie wywo³ana po naciœniêciu przycisku
        myButton.onClick.AddListener(ButtonClick);

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

    public void GetValX(string value1)
    {
        if(int.TryParse(value1, out int newvalue1))
        {
            ValX = newvalue1;
        }

    }
    
    public void GetValY(string value2)
    {
        if (int.TryParse(value2, out int newvalue2))
        {
            ValY = newvalue2;
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

    void ButtonClick()
    {
       
        if (OnIntValueUpdated != null)
        {
            OnIntValueUpdated(ValX, ValY);
        }
    }
}

