using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class MoveToTargetAgent : Agent
{
    [SerializeField] private Transform env;
    [SerializeField] private Transform target;
    [SerializeField] private MeshRenderer background;
    private float dist;

    public override void OnEpisodeBegin()
    {
        transform.localPosition = new Vector3(Random.Range(-3.5f,-1.5f), 0.56f, Random.Range(-3.5f, 3.5f));
        //target.localPosition = new Vector3(Random.Range(1.5f, 3.5f), 0.56f, Random.Range(-3.5f, 3.5f));
        
        //env.rotation = Quaternion.Euler(0, Random.Range(0, 360f), 0);
        transform.rotation = Quaternion.identity;

        dist = Vector3.Distance(target.localPosition, transform.localPosition);
    }
    //Metoda do okreslenia celu jaki ma osiagnac AI
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation((Vector3)transform.localPosition); //pozycja AI
        sensor.AddObservation((Vector3)target.localPosition); //pozycja celu
    }
    //Metoda do okreslenia jakich akcji ma sie podjac AI by osiagnac cel
    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];  //akcja przesuniecia sie po X
        float moveZ = actions.ContinuousActions[1]; //akcja przesuniecia sie po Y

        float movementSpeed = 5f;

        transform.localPosition += new Vector3(moveX, 0, moveZ) * Time.deltaTime * movementSpeed;

        float distTemp = Vector3.Distance(target.localPosition, transform.localPosition);
        // przyznawanie nagrod i kar za zmiane dystansu do celu
        if (distTemp < dist)
        {
            dist = distTemp;
            AddReward(0.1f);
        }
        else if (distTemp > dist)
        {
            dist = distTemp;
            AddReward(-0.1f);
        }
        // Sprawdzenie, czy agent jest zbyt blisko �ciany
        if (IsAgentTooCloseToWall())
        {
            AddReward(-0.2f * Time.fixedDeltaTime); // Kara za zbyt bliskie podej�cie do �ciany
        }

        // Kary za d�ugotrwa�e dzia�ania
        AddReward(-1f / MaxStep);


    }
    //Metoda ktora pozwala uzytkownikowi sterowac dzialaniami w celu testu 
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continousActions = actionsOut.ContinuousActions;
        continousActions[0] = Input.GetAxisRaw("Horizontal");
        continousActions[1] = Input.GetAxisRaw("Vertical");


    }
    private void OnTriggerEnter(Collider collision)
    {
        if(collision.TryGetComponent(out Target target))
        {
            AddReward(1f);
            background.material.color = Color.green;
            EndEpisode();
        }
        else if (collision.TryGetComponent(out Walls walls))
        {
            AddReward(-0.5f);
            background.material.color = Color.red;
            EndEpisode();
        }
    }

    private bool IsAgentTooCloseToWall()
    {
        // Dostosuj odleg�o�� od �ciany, kt�ra jest akceptowalna
        float acceptableDistance = 0.5f;

        // Sprawd� odleg�o�� od �ciany
        return Physics.Raycast(transform.position, transform.forward, acceptableDistance);
    }
}

