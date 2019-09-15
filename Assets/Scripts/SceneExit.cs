using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Storm.Player;

public class SceneExit : MonoBehaviour
{
    public string sceneName;

    public string spawnName;

    public void OnTriggerStay2D(Collider2D collider) {
        if(collider.CompareTag("Player")) {
            if(Input.GetKeyDown(KeyCode.UpArrow)) {
                var manager = GameController.Instance;
                manager.transitions.MakeTransition(sceneName, spawnName);
            }
        }
    }
}
