using UnityEngine;
using Unity.MLAgents;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class MazeAcademy : MonoBehaviour
{

    private Transform target;
    private Transform agent;
    private Renderer[] floors;
    private Color prevColors;

    [SerializeField] GameObject floorGameObject;
    [SerializeField] float movementSpeed = 5.0f;

    private void Start()
    {
        floors = floorGameObject.GetComponentsInChildren<Renderer>();
        prevColors = floorGameObject.GetComponentInChildren<Renderer>().material.color;
        target = GameObject.FindGameObjectWithTag("Target").GetComponent<Transform>();
        agent = GameObject.FindGameObjectWithTag("Agent").GetComponent<Transform>();

    }

    public (Vector3 position, Quaternion rotation) GetStartPosition()
    {
        Vector3 startPosition = new Vector3(Random.Range(-3.5f, -1.5f), 0.56f, Random.Range(-3.5f, 3.5f));
        Quaternion startRotation = Quaternion.identity; 
        return (startPosition, startRotation);
    }

    public Vector3 GetTargetPosition()
    {
        return target.localPosition;
    }

    public float GetDistanceToTarget()
    {
        return Vector3.Distance(agent.localPosition, target.localPosition);
    }

    public float GetMovementSpeed() { return movementSpeed; }

    public void OnEpisodeEnd(string reason)
    {
        switch(reason)
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
        //var prevColors = floors.Select(t => t.material.color).ToList();

        foreach (Renderer t in floors)
        {
            t.material.color = color;
        }

        yield return new WaitForSeconds(0.2f);

        for (int i = 0; i < floors.Count(); i++)
        {
            floors[i].material.color = prevColors;
        }
    }
}