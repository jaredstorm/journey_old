using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class EventZone : MonoBehaviour
{
    public UnityEvent events;

    public void OnTriggerEnter2D(Collider2D col) {
        if (col.gameObject.CompareTag("Player")) {
            events.Invoke();
        }
    }
}
