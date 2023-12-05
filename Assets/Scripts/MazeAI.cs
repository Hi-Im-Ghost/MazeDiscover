using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class MazeAI : Agent
{
    MazeAcademy env;

    private Rigidbody agentRigibody;
    private float StartDistanceToTarget = 0f;
    private float actualDistanceToTarget = 0f;
    private float bestDistanceToTarget = 0f;

    private void Start()
    {
        agentRigibody = GetComponent<Rigidbody>();
        env = FindObjectOfType<MazeAcademy>();
    }

    // Metoda do resetowania srodowiska szkoleniowego
    private void ResetEnvironment()
    {
        var start = env.GetStartPosition();
        StartDistanceToTarget = env.GetDistanceToTarget();

        transform.localPosition = start.position;
        transform.localRotation = start.rotation;
        agentRigibody.velocity = Vector3.zero;
        agentRigibody.angularVelocity = Vector3.zero;
        
    }

    public override void OnEpisodeBegin()
    {
        ResetEnvironment();

        //transform.localPosition = new Vector3(Random.Range(-3.5f,-1.5f), 0.56f, Random.Range(-3.5f, 3.5f));

        actualDistanceToTarget = StartDistanceToTarget;
    }
    // Metoda do okreslenia celu jaki ma osiagnac AI

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation((Vector3)this.transform.localPosition); //pozycja AI
        sensor.AddObservation(env.GetTargetPosition()); //pozycja celu
    }

    // Metoda do obliczania nagrod/kar
    private void CalculateRewards()
    {
        // Aktualizacja najlepszego dystansu
        if (actualDistanceToTarget < bestDistanceToTarget)
        {
            bestDistanceToTarget = actualDistanceToTarget;
            // Nagroda za poprawê najlepszego dystansu
            AddReward(0.05f);
        }
        
        // Poprawa dystansu
        if (actualDistanceToTarget < StartDistanceToTarget)
        {
            // Nagroda za poprawe sytuacji
            AddReward(0.005f);
        }

        // Sprawdzenie czy nie przekroczono maksymalnej ilosci akcji
        if (StepCount >= MaxStep)
        {
            env.OnEpisodeEnd("time");
        }

        // Kary za d³ugotrwale dzia³ania
        AddReward(-0.0005f);
    }

    //Metoda do okreslenia jakich akcji ma sie podjac AI by osiagnac cel
    public override void OnActionReceived(ActionBuffers actions)
    {
        float forwardAmount = 0f; //ruch przod tyl
        float turnAmount = 0f; //skret

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
        transform.Rotate(Vector3.up, turnAmount * Time.fixedDeltaTime * env.GetMovementSpeed() * 20f);

        // Przesuniêcie agenta
        transform.localPosition += transform.forward * forwardAmount * Time.fixedDeltaTime * env.GetMovementSpeed();


        actualDistanceToTarget =env.GetDistanceToTarget();

        CalculateRewards();

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
            env.OnEpisodeEnd("hit");
            EndEpisode();

        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Target"))
        {
            AddReward(1f);
            env.OnEpisodeEnd("target");
            EndEpisode();
        }
    }
}

