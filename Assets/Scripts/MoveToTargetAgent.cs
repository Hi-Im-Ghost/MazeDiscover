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
        //dist = Vector3.Distance(target.localPosition, transform.localPosition);
    }
    //Metoda do okreslenia celu jaki ma osiagnac AI

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation((Vector3)transform.localPosition); //pozycja AI
        sensor.AddObservation((Vector3)target.localPosition); //pozycja celu
        //sensor.AddObservation(dist); //dystans
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
        transform.Rotate(Vector3.up, turnAmount * Time.fixedDeltaTime * movementSpeed * 20f);

        // Przesuniêcie agenta
        transform.localPosition += transform.forward * forwardAmount * Time.fixedDeltaTime * movementSpeed;
        
        
        // Kary za d³ugotrwa³e dzia³ania
        AddReward(-0.0005f);


    }
    //Metoda ktora pozwala uzytkownikowi sterowac dzialaniami w celu testu 
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;
        discreteActions[0] = 0; // Ruch przód/ty³
        discreteActions[1] = 0; // Skrêt

        // ruch przód/tyl
        if (Input.GetKey(KeyCode.UpArrow))
        {
            discreteActions[0] = 1; // Ruch przód
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            discreteActions[0] = 2; // Ruch ty³
        }

        // skrêt
        if (Input.GetKey(KeyCode.RightArrow))
        {
            discreteActions[1] = 1; // Skrêt w lewo
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            discreteActions[1] = 2; // Skrêt w prawo
        }


    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            AddReward(-1f);
            background.material.color = Color.red;
            EndEpisode();

        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Target"))
        {
            AddReward(1f);
            background.material.color = Color.green;
            EndEpisode();
        }
    }
}

