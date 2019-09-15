using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public SpawnPoint spawn;

    public void Start() {
        if (spawn == null) {
            spawn = GetComponentInChildren<SpawnPoint>();
        }
    }

    public void OnTriggerEnter2D(Collider2D col) {
        //Debug.Log("Name: " + name);
        if (col.CompareTag("Player")) {
            GameController.Instance.transitions.setCurrentSpawn(spawn.spawnName);
        }
    }
    
}
