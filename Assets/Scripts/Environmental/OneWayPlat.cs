using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Storm.Player;

public class OneWayPlat : MonoBehaviour
{

    Collider2D physicsCollider;
    Collider2D triggerCollider;

    private bool playerIsTouching;

    private float disableTimer;
    public float disabledTime;

    void Start() {
        var colliders = GetComponents<Collider2D>();
        physicsCollider = colliders[0];
        triggerCollider = colliders[1];
    }

    void Update() {
        if (playerIsTouching && Input.GetKey(KeyCode.DownArrow)) {
            DisablePlatform();
            disableTimer = disabledTime;
        } 
    
        if (!physicsCollider.enabled) {
            disableTimer -= Time.deltaTime;
        } 
        
        if (disableTimer <= 0) {
            EnablePlatform();
            disableTimer = 0;
        }

    }

    void DisablePlatform() {
        physicsCollider.enabled = false;
        triggerCollider.enabled = false;
    }

    void EnablePlatform() {
        physicsCollider.enabled = true;
        triggerCollider.enabled = true;
    }

    void OnTriggerEnter2D(Collider2D other) {
        Debug.Log("OnTriggerEnter");
        if (other.CompareTag("Player")) {
            Debug.Log("Found Player!");
            PlayerCharacter player = other.GetComponent<PlayerCharacter>();

            // if Player is below the platform.
            Vector3 direction = transform.position - player.transform.position;
            Debug.Log("Y VAL: " + direction.y);
            if (direction.y >= 0) {
                Debug.Log("Collision!");
                DisablePlatform();
                disableTimer = disabledTime;
            }
        }
    }


    

    void OnCollisionEnter2D(Collision2D collision) {
        if (collision.collider.CompareTag("Player")) {
            playerIsTouching = true;
        }
    }

    void OnCollisionExit2D(Collision2D collision) {
        if (collision.collider.CompareTag("Player")) {
            playerIsTouching = false;
        }
    }
 }
