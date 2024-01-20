using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Policies;
using Unity.VisualScripting;

public class MazeAI : Agent
{
    MazeAcademy env;

    private TrailRenderer trailRenderer;
    private Rigidbody agentRigibody;
    private float StartDistanceToTarget = 0f;
    private float actualDistanceToTarget = 0f;
    private float bestDistanceToTarget = 0f;
    private BehaviorParameters bp;
    private DecisionRequester dr;
    private RayPerceptionSensorComponent3D raySensor;

    // Metoda do ustawiania parametrów uczenia
    private void setLearningParams()
    {
        // BEHAVIOR PARAMETERS
        bp.BehaviorName = "MoveToTarget";
        bp.BrainParameters.VectorObservationSize = 6;
        bp.BrainParameters.NumStackedVectorObservations = 1;
        bp.BrainParameters.ActionSpec = ActionSpec.MakeDiscrete(3, 3);
        bp.InferenceDevice = InferenceDevice.Default;
        bp.UseChildSensors = false;
        bp.ObservableAttributeHandling = ObservableAttributeOptions.Ignore;
        MaxStep = 10000;

        // DECISION REQUESTER
        dr.DecisionPeriod = 10;
        dr.TakeActionsBetweenDecisions = true;

        // RAY SENSOR
        raySensor.SensorName = "RayPerceptionSensor";
        raySensor.DetectableTags.Add("Wall");
        raySensor.DetectableTags.Add("Target");
        raySensor.RaysPerDirection = 3;
        raySensor.MaxRayDegrees = 90;
        raySensor.RayLength = 8;
        raySensor.ObservationStacks = 1;
        raySensor.StartVerticalOffset = 0;
        raySensor.EndVerticalOffset = 0;

    }
    // Metoda do ustawienia komponentu Rigibody
    private void setRigibody()
    {
        // Wy³¹czenie grawitacji
        agentRigibody.useGravity = false;
        // Zablokowanie poruszania i rotacji wzd³ó¿ danej wspó³rzednej
        agentRigibody.constraints = RigidbodyConstraints.FreezePositionY;
        agentRigibody.constraints = RigidbodyConstraints.FreezeRotation;
    }

    // Metoda do resetowania agenta
    private void ResetAgent()
    {
        // Zapisanie w zmienna zwracana pozycje startowa agenta
        var start = env.GetStartPosition();
        // Zapisanie w zmienna zwracany dystans do celu
        StartDistanceToTarget = env.GetDistanceToTarget();
        // Ustawienie pozycji 
        transform.localPosition = start.position;
        // Ustawienie rotacji
        transform.localRotation = start.rotation;
        // Ustawienie prêdkoœci
        agentRigibody.velocity = Vector3.zero;
        // Ustawienie predkosci katowej
        agentRigibody.angularVelocity = Vector3.zero;
        // Zresetowanie pozostawianego œladu
        ResetTrail();
    }
    // Metoda wywo³ywana podczas inicjalizacji
    public override void Initialize()
    {
        // Wywo³aj metode bazowa
        base.Initialize();
        // Pobierz komponent TrailRenderer i zapisz w zmienn¹
        trailRenderer = GetComponentInChildren<TrailRenderer>();
        // Pobierz komponent Rigibody i zapisz w zmienn¹
        agentRigibody = this.gameObject.AddComponent<Rigidbody>();
        // Dodaj do tego obiektu komponent BoxColider
        this.gameObject.AddComponent<BoxCollider>();
        // Znajdz obiekt typu MazeAcademy i zapisz do zmiennej
        env = FindObjectOfType<MazeAcademy>();
        // Zapisz do zmiennej komponent BehaviorParameters
        bp = GetComponent<BehaviorParameters>();
        // Zapisz do zmiennej komponent DecisionRequester
        dr = GetComponent<DecisionRequester>();
        // Zapisz do zmiennej komponent RayPerceptionSensorComponent3D
        raySensor = GetComponent<RayPerceptionSensorComponent3D>();
        // Wywo³aj metode setRigibody
        setRigibody();
        // Wywo³aj metode do ustawienia parametrów uczenia
        //setLearningParams();
    }
    // Metoda wywo³ywana z momencie rozpoczecia epizodu 
    public override void OnEpisodeBegin()
    {
        // Wywo³aj metode klasy bazowej
        base.OnEpisodeBegin();
        // Znajdz obiekt typu mazegenerator i wywo³aj metode generateNewMaze
        FindObjectOfType<MazeGenerator>().GenerateNewMaze();
        // Wywo³aj metode do resetowania ustawieñ agenta 
        ResetAgent();
        // Przypisz poczatkowy dystans do celu jako aktualny
        actualDistanceToTarget = StartDistanceToTarget;

    }

    // Metoda do okreslenia celu jaki ma osiagnac AI
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(this.transform.localPosition); //pozycja AI
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
            AddReward(0.006f);
        }
        
        // Poprawa dystansu
        if (actualDistanceToTarget < StartDistanceToTarget)
        {
            // Nagroda za poprawe sytuacji
            AddReward(0.0006f);
        }

        // Sprawdzenie czy nie przekroczono maksymalnej ilosci akcji
        if (StepCount >= MaxStep)
        {
            // Wywo³aj poni¿sza metode z MazeAcademy z parametrem time
            env.OnEpisodeEnd("time");
            // Przyznaj nagrode -1 
            SetReward(-1f);
            // Zakoñcz epizod
            EndEpisode();
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

        // Obliczenie aktualnego dystansu do celu i przypisanie do zmiennej
        actualDistanceToTarget = env.GetDistanceToTarget();

        // Wywo³anie metody, która przydzieli odpowiednie nagrody
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
    // Metoda do resetowania œladu 
    private void ResetTrail()
    {
        trailRenderer.Clear();
        trailRenderer.enabled = true;
    }
    // Wywo³ywane w momencie kolizji z obiektem z ustawionym triggerem
    private void OnTriggerEnter(Collider collision)
    {
        // Jeœli jest to Cel to..
        if (collision.gameObject.CompareTag("Target"))
        {
            // Daj 2 punkty nagrody
            AddReward(2f); 
            // Wywo³aj poni¿sza metode z MazeAcademy z parametrem target
            env.OnEpisodeEnd("target");
            // Zakoncz epizod 
            EndEpisode();
        }
        // Jeœli to œciana to ..
        if (collision.gameObject.CompareTag("Wall"))
        {
            // Przyznaj -1 nagrody
            SetReward(-1f);
            // Wywo³aj poni¿sza metode z MazeAcademy z parametrem hit
            env.OnEpisodeEnd("hit");
            // Zakoñcz epizod
            EndEpisode();

        }
    }
}

