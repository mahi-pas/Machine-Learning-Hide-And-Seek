using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class Hider : PlatformingAgent
{
    private bool found;

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
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if( other.gameObject.tag == "Seeker"){
            AddReward(-100f);
            EndEpisode();
        }
    }
}
