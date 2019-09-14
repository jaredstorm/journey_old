using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour {

    public string spawnName;

    public bool isFacingRight;

    // Start is called before the first frame update
    void Awake() {
        GameController.Instance.transitions.RegisterSpawn(spawnName, transform.position, isFacingRight);
    }

}
