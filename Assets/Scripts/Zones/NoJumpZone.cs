using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Storm.Player;

public class NoJumpZone : MonoBehaviour {
    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            other.GetComponent<PlayerCharacter>().movement.DisableJump();
        }
    }

    public void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            other.GetComponent<PlayerCharacter>().movement.EnableJump();
        }
    }
}
