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
    private SuccessCounter counter;
    private GameObject agentInst;

    [SerializeField] GameObject agentPrefab;
    [SerializeField] float movementSpeed = 5.0f;


    private void Start()
    {

    }

    public void generateAgent()
    {
        agentInst = Instantiate(agentPrefab, spawnPoint.transform.localPosition, Quaternion.identity, transform);
        SetAgentTransform(agentInst.transform);
        prevColors = agentInst.GetComponentInChildren<Renderer>().material.color;
        counter = FindObjectOfType<SuccessCounter>();
    }
    public (Vector3 position, Quaternion rotation) GetStartPosition()
    {
        Vector3 startPosition = spawnPoint.localPosition;
        Quaternion startRotation = Quaternion.identity; 
        return (startPosition, startRotation);
    }


    public void setStartPosition(Transform startPoint)
    {
        this.spawnPoint = startPoint;
    }
    public void SetAgentTransform(Transform agentObject)
    {
        this.agentTransform = agentObject;
        
    }

    public void SetTargetTransform(Transform targetObject)
    {
        this.targetTransform = targetObject;
    }

    public Vector3 GetTargetPosition()
    {
        return targetTransform.localPosition;
    }

    public float GetDistanceToTarget()
    {
        return Vector3.Distance(agentTransform.localPosition, targetTransform.localPosition);
    }

    public float GetMovementSpeed() { return movementSpeed; }


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


    IEnumerator ChangeColorBg(Color color)
    {
        agentInst.GetComponentInChildren<Renderer>().material.color = color;
        yield return new WaitForSeconds(0.4f);
        agentInst.GetComponentInChildren<Renderer>().material.color = prevColors;


    }
}