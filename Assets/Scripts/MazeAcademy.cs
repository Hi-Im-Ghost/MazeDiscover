using UnityEngine;
using Unity.MLAgents;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;

public class MazeAcademy : MonoBehaviour
{

    private Transform targetTransform;
    private Transform agentTransform;
    private Transform spawnPoint;

    private Color prevColors;
    private Widget counter;
    private GameObject agentInst;

    [SerializeField] GameObject agentPrefab;
    [SerializeField] float movementSpeed = 5.0f;

    // Metoda do tworzenia agenta
    public void generateAgent()
    {
        // Inicjalizacja agenta 
        agentInst = Instantiate(agentPrefab, spawnPoint.transform.localPosition, Quaternion.identity, transform);
        // Ustawienie polozenia agenta
        SetAgentTransform(agentInst.transform);
        // Pobranie koloru agentu i zapisanie w zmiennej
        prevColors = agentInst.GetComponentInChildren<Renderer>().material.color;
        // Pobranie obiektu typu widget i zapisanie w zmiennej
        counter = FindObjectOfType<Widget>();
    }
    // Metoda do zwracania pozycji startowej agenta
    public (Vector3 position, Quaternion rotation) GetStartPosition()
    {
        Vector3 startPosition = spawnPoint.localPosition;
        Quaternion startRotation = Quaternion.identity; 
        return (startPosition, startRotation);
    }

    // Metoda do ustawienia miejsca startowego dla agenta
    public void setStartPosition(Transform startPoint)
    {
        this.spawnPoint = startPoint;
    }
    // Metoda do ustawienia polozenia agenta
    public void SetAgentTransform(Transform agentObject)
    {
        this.agentTransform = agentObject;
        
    }
    // Metoda do ustawienia polozenia celu
    public void SetTargetTransform(Transform targetObject)
    {
        this.targetTransform = targetObject;
    }
    // Metoda do zwrócenia pozycji celu
    public Vector3 GetTargetPosition()
    {
        return targetTransform.localPosition;
    }
    // Metoda do zwracania dystansu do celu
    public float GetDistanceToTarget()
    {
        return Vector3.Distance(agentTransform.localPosition, targetTransform.localPosition);
    }
    // Metoda do zwracania predkosci poruszania agenta
    public float GetMovementSpeed() { return movementSpeed; }

    // Metoda do obs³ugi koñca epizodu
    public void OnEpisodeEnd(string reason)
    {
        switch (reason)
        {
            case "target":
                {
                    StartCoroutine("ChangeColorBg", Color.green);
                    counter.IncrementSuccessCount();
                }
                break;
            case "hit":
                {
                    StartCoroutine("ChangeColorBg", Color.red);
                    counter.IncrementFailureCount();
                }
                break;
            case "time":
                {
                    StartCoroutine("ChangeColorBg", Color.yellow);
                    counter.IncrementTimeoutCount();
                }
                break;
        }
    }

    // Metoda wspó³programowa do zmiany koloru
    IEnumerator ChangeColorBg(Color color)
    {
        agentInst.GetComponentInChildren<Renderer>().material.color = color;
        // Wstrzymanie wykonywania na podana ilosc sekund 
        yield return new WaitForSeconds(0.4f);
        agentInst.GetComponentInChildren<Renderer>().material.color = prevColors;
    }
}