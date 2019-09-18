using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Storm.Characters.Player {
    public class PlayerMovementRealistic : PlayerMovement {
    
        protected void FixedUpdate() {
            jumpCalculations();
            moveCalculations();
        }
    }
}
