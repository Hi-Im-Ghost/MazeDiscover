using UnityEngine;
using Unity.MLAgents;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class MazeAcademy : MonoBehaviour
{

    private Transform targetTransform;
    private Transform agentTransform;
    private Transform spawnPoint;

    private Renderer[] floors;
    private Color prevColors;

    private GameObject floorGameObject;
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
    }
    public (Vector3 position, Quaternion rotation) GetStartPosition()
    {
        Vector3 startPosition = spawnPoint.localPosition;
        Quaternion startRotation = Quaternion.identity; 
        return (startPosition, startRotation);
    }

    public void SetFloorGameObject(GameObject floor)
    {
        this.floorGameObject = floor;
        this.floors = floorGameObject.GetComponentsInChildren<Renderer>();
        prevColors = floorGameObject.GetComponentInChildren<Renderer>().material.color;
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
                }
                break;
            case "hit":
                {
                    StartCoroutine("ChangeColorBg", Color.red);
                }
                break;
            case "time":
                {
                    StartCoroutine("ChangeColorBg", Color.yellow);
                }
                break;
        }
    }


    IEnumerator ChangeColorBg(Color color)
    {
        //foreach (Renderer t in floors)
        //{
        //    t.material.color = color;
        //}

        //yield return new WaitForSeconds(0.1f);

        //for (int i = 0; i < floors.Count(); i++)
        //{
        //    floors[i].material.color = prevColors;
        //}  

        agentInst.GetComponentInChildren<Renderer>().material.color = color;
        yield return new WaitForSeconds(0.4f);
        agentInst.GetComponentInChildren<Renderer>().material.color = prevColors;


    }
}