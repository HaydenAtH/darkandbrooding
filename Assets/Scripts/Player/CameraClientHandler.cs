using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;
using Unity.Netcode;

public class CameraClientHandler : NetworkBehaviour {
    public Camera cam;
    public override void OnNetworkSpawn(){
        
        // if this spawns in owned by the local player
        if (!IsOwner)
        {
            // disable the camera
            cam.enabled = false;
            // disable the audio listener
            cam.gameObject.GetComponent<AudioListener>().enabled = false;
        }else{

        }
        // ...

    }
}