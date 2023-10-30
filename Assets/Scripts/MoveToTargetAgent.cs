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

    public override void OnEpisodeBegin()
    {
        transform.localPosition = new Vector3(Random.Range(-3.5f,-1.5f),0f, Random.Range(-3.5f, 3.5f));
        target.localPosition = new Vector3(Random.Range(1.5f, 3.5f), 0f, Random.Range(-3.5f, 3.5f));
        
        env.rotation = Quaternion.Euler(0, Random.Range(0, 360f), 0);
        transform.rotation = Quaternion.identity;
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation((Vector3)transform.localPosition);
        sensor.AddObservation((Vector3)target.localPosition);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];

        float movementSpeed = 5f;

        transform.localPosition += new Vector3(moveX, 0, moveZ) * Time.deltaTime * movementSpeed;
    }

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
            AddReward(10f);
            background.material.color = Color.green;
            EndEpisode();
        }
        else if (collision.TryGetComponent(out Walls walls))
        {
            AddReward(-2f);
            background.material.color = Color.red;
            EndEpisode();
        }
    }
}