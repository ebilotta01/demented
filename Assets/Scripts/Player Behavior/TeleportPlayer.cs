using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.AI;

public class TeleportPlayer : MonoBehaviour
{
    [SerializeField]
    public Text portalText;

    bool withinRange = false;
    PlayerController player;
    NavMeshAgent boss;
    // Update is called once per frame
    private void Start() {
        portalText.text = "";
    }

    private void Update() {
        if(withinRange && player.interactAction.triggered){
            StartBossEncounter();
        }
    }    

    public void StartBossEncounter() {
        // compute a random point in the boss room to teleport the player to
        Vector3 pt = new Vector3(Level.BossRoom.Center.x, 6, Level.BossRoom.Center.z);
        float angle = Random.Range(0.0f, 2.0f * Mathf.PI);
        pt.x += Level.RoomSize * Mathf.Cos(angle) / 3.0f;
        pt.z += Level.RoomSize * Mathf.Sin(angle) / 3.0f;

        // teleport the player to the boss room
        player.teleport(pt);
        withinRange = false;
        Player.inBossRoom = true;
    }

    private void OnTriggerStay(Collider other) {
        // Debug.Log(other.gameObject.tag);
        if(other.gameObject.tag == "Player" && Level.TotalEnemies == 0){ // set to > 0 for testing, should be == 0 for gameplay
            withinRange = true;
            player = other.gameObject.GetComponent<PlayerController>();
            // other.gameObject.GetComponent<PlayerController>().teleport(Level.BossRoom.Center);d
        } else if(other.gameObject.tag == "Player") {
            portalText.text = ("There are still " + Level.TotalEnemies + " Enemies left");
        }
    }
}
