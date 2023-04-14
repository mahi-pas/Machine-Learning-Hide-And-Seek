using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class Seeker : PlatformingAgent
{

    public override void OnEpisodeBegin()
    {   
        base.OnEpisodeBegin();
        closestDistance = Vector2.Distance(transform.position,enemy.transform.position);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        base.OnActionReceived(actions);
        float newDistance = Vector2.Distance(transform.position,enemy.transform.position);
        if(newDistance < closestDistance){
            AddReward(0.01f);
            //Debug.Log("Seeker: Reward + 0.01f");
            closestDistance = newDistance;
        }
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if( other.gameObject.tag == "Hider"){
            AddReward(100f);
            background.color = winColor;
            EndEpisode();
        }
    }

}
