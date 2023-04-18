using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class Seeker : MovementAgent
{
    public Transform goal;

    public override void OnEpisodeBegin()
    {   
        base.OnEpisodeBegin();
        closestDistance = Vector2.Distance(transform.position,enemy.transform.position);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        base.CollectObservations(sensor);
        sensor.AddObservation(goal.transform.position);
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if( other.gameObject.tag == "Hider"){
            Debug.Log("Seeker: End Episode");
            AddReward(100f + CalculateAdditionalReward());
            background.color = winColor;
            EndEpisode();
        }
    }

}
