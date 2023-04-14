using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class Hider : PlatformingAgent
{
    private bool found;
    private int goalsReached;

    [Header("Hider")]
    public Transform goal;

    public override void Awake() {
        base.Awake();
        found = true;
    }

    public override void OnEpisodeBegin()
    {   
        base.OnEpisodeBegin();
        if(!found){
            background.color = winColor;
        }
        found = false;
        goalsReached = 0;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        base.CollectObservations(sensor);
        sensor.AddObservation(goal.transform.position);
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if( other.gameObject.tag == "Seeker"){
            found = true;
            AddReward(-100f);
            EndEpisode();
        }
        if(other.gameObject.tag == "Goal"){
            goalsReached+=1;
            if(goalsReached == 5){
                AddReward(100f);
                EndEpisode();
            }
            else{
                AddReward(10f);
            }
        }
    }
}
