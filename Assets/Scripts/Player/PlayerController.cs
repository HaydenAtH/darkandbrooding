using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Experimental.GlobalIllumination;

public class PlayerController : NetworkBehaviour
{
    public Rigidbody rb;
    public float speed = 5f;
    public float drag = 1;
    public Vector2 sensitivity = new Vector2(1, 1);
    public GameObject cameraHandler;
    public GameObject playerObject;
    public GameObject muzzleFlash;
    public float deathCooldown;
    private Vector2 impulse;
    private bool isDead;
    float xRot, yRot;
    private Vector3 camOffset;

    private List<Vector3> spawns;
    
    // ----------------- //
    //  Network Variables //
    // ----------------- //

    
    private NetworkVariable<int> hp = new NetworkVariable<int>(100, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private NetworkVariable<int> team = new NetworkVariable<int>(-1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public override void OnNetworkSpawn() {
        gameObject.name = OwnerClientId.ToString();
        if(IsOwner) {
            camOffset = cameraHandler.transform.position - this.transform.position;
            print("Joined");
            muzzleFlash.SetActive(false);
            lockMouse();

            if (!rb) rb = GetComponent<Rigidbody>();
            //rb.freezeRotation = true;

            hp.OnValueChanged += onHpChanged;
        }
    }

    public override void OnNetworkDespawn() {
        hp.OnValueChanged -= onHpChanged;
    }

    void onHpChanged(int previous, int current) {
        hp.Value = current;
        print(OwnerClientId + " : " + current);
        if (current < 0 && !isDead) {
            // Death Logic
            
            // Start respawn countdown
            StartCoroutine(deathCountdown());
            // Disconnect camera and playercontroller
            cameraHandler.transform.parent = null;
            // set player location to deathbox
            this.transform.position = new Vector3(-81.6900024f, 6.88000011f, 23.1700001f);
            
        }
    }

    private IEnumerator deathCountdown() {
        yield return new WaitForSeconds(deathCooldown);
        ServerManager.Singleton.getNewSpawnServerRpc(OwnerClientId, team.Value);
    }

    // Update is called once per frame
    void Update() {
        Debug.Log( OwnerClientId + " HP: " + hp.Value);
        if (!IsOwner) { return; }

        if (team.Value == -1) {
            ServerManager.Singleton.assignTeamServerRpc(OwnerClientId);
        }
        
        float mouseX = Input.GetAxisRaw("Mouse X") * sensitivity.x * Time.deltaTime;
        float mouseY = Input.GetAxisRaw("Mouse Y") * sensitivity.y * Time.deltaTime;

        yRot += mouseX;
        xRot -= mouseY;
        xRot = Mathf.Clamp(xRot, -90f, 90f);
        cameraHandler.transform.localRotation = Quaternion.Euler(xRot, 0f, 0f);
        playerObject.transform.localRotation = Quaternion.Euler(0f, yRot, 0f);
        transform.localRotation = Quaternion.Euler(0f, yRot, 0f);

        calcImpulse();
        applyImpulse();

        if(Input.GetMouseButtonDown(0)){
            StartCoroutine(fire());
        }

        if (Input.GetKeyDown(KeyCode.T)) {
            hp.Value = -1;
        }
    }

    void lockMouse(){
        Cursor.lockState = CursorLockMode.Locked;
    }

    void calcImpulse() {
        impulse.y = Input.GetAxisRaw("Vertical");
        impulse.x = Input.GetAxisRaw("Horizontal");
    }

    void applyImpulse(){
        Vector3 moveDir = Vector3.zero;
        moveDir = (transform.forward * impulse.y) + (transform.right * impulse.x);
        
        rb.AddForce(moveDir.normalized * speed * Time.deltaTime, ForceMode.Impulse);

        if (impulse == Vector2.zero) {
            rb.drag = 6;
        } else {
            rb.drag = drag;
        }
        
        Vector3 flatVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if (flatVelocity.magnitude > speed) {
            Vector3 l = flatVelocity.normalized * speed;
            rb.velocity = new Vector3(l.x, rb.velocity.y, l.z);
        }
        
    }

    [ClientRpc]
    public void damageClientRpc(int f) {
        if (!IsOwner) return;
        hp.Value -= f;
    }
    
    [ClientRpc]
    public void setTeamClientRpc(int t) {
        if (!IsOwner) return;
        
        print(OwnerClientId + " assigned to team " + t);
        team.Value = t;
    }

    [ClientRpc]
    public void setSpawnClientRpc(float x, float y, float z) {
        // Respawn logic
        print(OwnerClientId + " Respawning");
        cameraHandler.transform.position = camOffset + transform.position;
        cameraHandler.transform.parent = transform;
        
        
        this.transform.position = new Vector3(x, y, z);
        hp.Value = 100;
    }

    IEnumerator fire(){
        Vector3 fwd = transform.TransformDirection(Vector3.forward);
        RaycastHit hit;

        if (Physics.Raycast(transform.position, fwd, out hit)){
            if (hit.rigidbody) {
                print(" Z Found Rigidbody");
                if (hit.rigidbody.gameObject.CompareTag("Player")) {
                    print(" Z Is Player");
                    ServerManager.Singleton.damagePlayerServerRpc( ulong.Parse(hit.rigidbody.gameObject.name), 10);
                }
            }
        }
        
        Debug.DrawRay(transform.position, fwd, Color.red);
    
        muzzleFlash.transform.localRotation = Quaternion.Euler(Random.Range(0f, 360f), -90f, 0f);
        muzzleFlash.SetActive(true);

        yield return new WaitForSeconds(0.1f);
       muzzleFlash.SetActive(false);
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void damageServerRpc() {
        hp.Value -= 10;
    }
}
