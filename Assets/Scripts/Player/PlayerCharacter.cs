using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace Storm.Player {
    public class PlayerCharacter : MonoBehaviour {

        public bool isDead = false;

        public PlayerMovement movement;

        public PlayerCollisionSensor sensor;

        public void Start() {
            if (sensor == null) {
                sensor = GetComponent<PlayerCollisionSensor>();
            }
        }
    }
}

