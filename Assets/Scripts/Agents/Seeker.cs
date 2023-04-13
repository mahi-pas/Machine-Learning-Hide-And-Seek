using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class Seeker : PlatformingAgent
{

    private void OnCollisionEnter2D(Collision2D other) {
        if( other.gameObject.tag == "Hider"){
            AddReward(100f);
            background.color = winColor;
            EndEpisode();
        }
    }

}
