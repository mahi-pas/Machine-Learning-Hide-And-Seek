using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class Hider : MovementAgent
{
    private bool found;
    private int goalsReached;

    [Header("Hider")]
    public Transform goal;
    public int goalsNeeded = 3;
    [SerializeField] List<Vector3> goalLocations;

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
        MoveGoal();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        base.CollectObservations(sensor);
        sensor.AddObservation(goal.transform.position);
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if( other.gameObject.tag == "Seeker"){
            found = true;
            SetReward(-100f);
            EndEpisode();
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        /*
        if(other.gameObject.tag == "Goal"){
            goalsReached+=1;
            if(goalsReached >= goalsNeeded){
                AddReward(100f + CalculateAdditionalReward());
                background.color = winColor;
                epCntl.EndEpisode(); //use episode controller to end episode for all agents
            }
            else{
                AddReward(50f + CalculateAdditionalReward());
                MoveGoal();
            }
        }
        */
    }

    private void MoveGoal(){
        goal.transform.localPosition = FindSpawnLocation(transform, minSpawnDist);
        closestDistance = Vector2.Distance(transform.position,goal.transform.position);
        timeSinceGoal = 0f;
    }
}
