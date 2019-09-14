using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Invisible : MonoBehaviour
{
    void Start() {
        GetComponent<SpriteRenderer>().enabled = false;
    }
}
