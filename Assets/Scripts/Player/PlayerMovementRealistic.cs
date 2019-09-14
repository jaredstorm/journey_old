using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Storm.Player {
    public class PlayerMovementRealistic : PlayerMovement {
    
        protected void FixedUpdate() {
            jumpCalculations();
            moveCalculations();
        }
    }
}
