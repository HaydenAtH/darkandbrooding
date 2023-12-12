using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TeamSpawn : NetworkBehaviour {
    public int team;

    public override void OnNetworkSpawn() {
        foreach (MeshRenderer r in GetComponentsInChildren<MeshRenderer>()) {
            r.enabled = false;
        }
    }
}
