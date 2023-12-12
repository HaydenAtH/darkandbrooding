using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class ServerManager : NetworkBehaviour {
    public static ServerManager Singleton;

    private int teamOneMembers = 0; // index = 0
    private int teamTwoMembers = 0; // index = 1

    private List<Vector3> teamOneSpawns = new List<Vector3>();
    private List<Vector3> teamTwoSpawns = new List<Vector3>();

    private void Start() {
        Singleton = this;
        
    }

    private void updateSpawnList() {
        teamOneSpawns.Clear();
        teamTwoSpawns.Clear();
        
        foreach (GameObject g in GameObject.FindGameObjectsWithTag("Spawn")) {
            if (g.name.Contains("0")) {
                teamOneSpawns.Add(g.transform.position);
            } else {
                teamTwoSpawns.Add(g.transform.position);
            }
        }
    }

    [ServerRpc]
    public void damagePlayerServerRpc(ulong id, int dmg) {
        GameObject.Find(id.ToString()).GetComponent<PlayerController>().damageClientRpc(dmg);
    }

    [ServerRpc]
    public void assignTeamServerRpc(ulong id) {
        print("Team One Members: " + teamOneMembers);
        print("Team Two Members: " + teamOneMembers);

        PlayerController p = GameObject.Find(id.ToString()).GetComponent<PlayerController>();

        if (teamOneMembers != teamTwoMembers) {
            if (teamOneMembers > teamTwoMembers) {
                p.setTeamClientRpc(1);
                teamTwoMembers++;
            } else {
                p.setTeamClientRpc(0);
                teamOneMembers++;
            }
        } else {
            p.setTeamClientRpc(0);
            teamOneMembers++;
        }
    }

    [ServerRpc]
    public void getNewSpawnServerRpc(ulong id, int team) {
        updateSpawnList();
        print("List: " + teamOneSpawns.Count);
        PlayerController p = GameObject.Find(id.ToString()).GetComponent<PlayerController>();

        Vector3 v;
        
        if (team == 0) {
            v = getRandomPos(teamOneSpawns);
        } else {
            v = getRandomPos(teamTwoSpawns);
        }
        
        p.setSpawnClientRpc(v.x, v.y, v.z);
    }

    private Vector3 getRandomPos(List<Vector3> l) {
        return l[Random.Range(0, l.Count)];
    }

    [ServerRpc]
    public void respawnServerRpc(ulong id, int dmg) {
        print("Respawning: " + id);
        //GameObject.Find(id.ToString()).GetComponent<PlayerController>().damageClientRpc(dmg);
    }
}
