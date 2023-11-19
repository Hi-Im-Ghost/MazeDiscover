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
        //transform.rotation = Quaternion.identity;
        dist = Vector3.Distance(target.localPosition, transform.localPosition);
    }
    //Metoda do okreslenia celu jaki ma osiagnac AI
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation((Vector3)transform.localPosition); //pozycja AI
        sensor.AddObservation((Vector3)target.localPosition); //pozycja celu
        sensor.AddObservation(dist); //dystans
    }
    //Metoda do okreslenia jakich akcji ma sie podjac AI by osiagnac cel
    public override void OnActionReceived(ActionBuffers actions)
    {
        float forwardAmount = 0f; //ruch przod tyl
        float turnAmount = 0f; //skret
        float movementSpeed = 5f; //predkosc

        switch (actions.DiscreteActions[0])
        {
            case 0: forwardAmount = 0f; break;
            case 1: forwardAmount = +1f; break;
            case 2: forwardAmount = -1f; break;
        }
        switch (actions.DiscreteActions[1])
        {
            case 0: turnAmount = 0f; break;
            case 1: turnAmount = +1f; break;
            case 2: turnAmount = -1f; break;
        }

        // Rotacja agenta
        transform.Rotate(Vector3.up, turnAmount * Time.deltaTime * movementSpeed * 20f);

        // Przesuni�cie agenta
        transform.localPosition += transform.forward * forwardAmount * Time.deltaTime * movementSpeed;

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

        // Kary za d�ugotrwa�e dzia�ania
        AddReward(-1f / MaxStep);


    }
    //Metoda ktora pozwala uzytkownikowi sterowac dzialaniami w celu testu 
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;
        discreteActions[0] = 0; // Ruch prz�d/ty�
        discreteActions[1] = 0; // Skr�t

        // ruch prz�d/tyl
        if (Input.GetKey(KeyCode.UpArrow))
        {
            discreteActions[0] = 1; // Ruch prz�d
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            discreteActions[0] = 2; // Ruch ty�
        }

        // skr�t
        if (Input.GetKey(KeyCode.RightArrow))
        {
            discreteActions[1] = 1; // Skr�t w lewo
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            discreteActions[1] = 2; // Skr�t w prawo
        }


    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<Target>(out Target target))
        {
            Debug.Log("yay");
            AddReward(-0.5f);
            //background.material.color = Color.red;
            //EndEpisode();
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<Walls>(out Walls walls))
        {
            Debug.Log("wow");
            AddReward(-0.1f);
            //EndEpisode();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Target>(out Target target))
        {
            AddReward(1f);
            background.material.color = Color.green;
            EndEpisode();
        }
    }
}

