using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestructing : MonoBehaviour
{
    public void SelfDestruct() {
        Destroy(this.gameObject);
    }
}
