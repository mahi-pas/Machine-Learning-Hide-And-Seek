using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;

public class EpisodeController : MonoBehaviour
{
    private List<Agent> agents;

    private void Awake() {
        agents = new List<Agent>();
    }

    public void AddAgent(Agent agent){
        agents.Add(agent);
    }

    public void EndEpisode(){
        foreach (Agent agent in agents)
        {
            agent.EndEpisode();
        }
    }

}
