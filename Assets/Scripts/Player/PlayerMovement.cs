using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Storm.Player {
    public abstract class PlayerMovement : MonoBehaviour {

        // ------------------------------------------------------------------------
        // Component Variables
        // ------------------------------------------------------------------------
        public Rigidbody2D rb;

        public Animator anim;

        protected PlayerCollisionSensor sensor;

        // ------------------------------------------------------------------------
        // Player Movement Variables 
        // ------------------------------------------------------------------------

        // The acceleration used in speeding up
        public float acceleration;

        // The player's max speed.
        public float maxVelocity;

        protected float maxSqrVelocity;

        // The deceleration applied to slowing down.
        public Vector2 deceleration;

        // Controls how fast the player turns around during movement.
        public float rebound;

        // ------------------------------------------------------------------------
        // Jumping Variables
        // ------------------------------------------------------------------------
        
        // The vertical force to apply to a jump.
        public float jump;
        
        protected Vector2 jumpForce;

        // ------------------------------------------------------------------------
        // Raycasting Variables
        // ------------------------------------------------------------------------

        // Measures the center of the player to the ground.
        protected float distanceToGround;

        // Determines how sensitive ground raycasting is.
        public float raycastBuffer;

        // Determines which layers to raycast on.
        public LayerMask groundLayerMask;

        // ------------------------------------------------------------------------
        // Player Orientation Information
        // ------------------------------------------------------------------------
        public bool isFacingRight;

        public bool isOnGround;

        // ------------------------------------------------------------------------
        // Mechanic Controls
        // ------------------------------------------------------------------------
        public bool isJumpingEnabled;

        public bool isMovingEnabled;

        #region Sensing Variables
        //---------------------------------------------------------------------------------------//
        //  Sensing Variables
        //---------------------------------------------------------------------------------------//
        private Vector3 bottomLeft;

        private Vector3 bottomRight;

        private Vector3 topLeft;

        private Vector3 topRight;
        #endregion



        // Start is called before the first frame update
        public virtual void Start() {
            var pos = GameController.Instance.transitions.getSpawnPosition();

            transform.position = GameController.Instance.transitions.getSpawnPosition();
            isFacingRight = GameController.Instance.transitions.getSpawningRight();
            anim.SetBool("IsFacingRight", isFacingRight);

            jumpForce = new Vector2(0, jump);
            maxSqrVelocity = maxVelocity*maxVelocity;
            rb = GetComponent<Rigidbody2D>();
            rb.freezeRotation = true;
            anim = GetComponent<Animator>();

            sensor = GetComponent<PlayerCollisionSensor>();
        }

        // ------------------------------------------------------------------------
        // Player Movement Hooks
        // ------------------------------------------------------------------------

        protected virtual void jumpCalculations() {
            RaycastHit2D hit = Physics2D.Raycast(
                transform.position, 
                Vector2.down, 
                distanceToGround, 
                groundLayerMask
            );
            
            if (hit.collider != null) {
                isOnGround = true;
            }

            // Jump if situationally appropriate.
            if (isJumpingEnabled && 
                isOnGround &&
                (Input.GetKeyDown(KeyCode.Space) || Input.GetKey(KeyCode.Space))) {

                rb.AddForce(jumpForce, ForceMode2D.Impulse);
                isOnGround = false;
            } 
        }

        protected virtual void moveCalculations() {
            // Move the player.
            if (!isMovingEnabled) return;

            float input = Input.GetAxis("Horizontal");

            // decelerate.
            if (Mathf.Abs(input) != 1 && isOnGround) { 
                rb.velocity *= deceleration; 
                return;
            }

            // Get player direction.
            float inputDirection = Mathf.Sign(input);
            float motionDirection = Mathf.Sign(rb.velocity.x);

            // If the player is turning around, apply more force
            float adjustedInput = inputDirection == motionDirection ? input : input*rebound;
            //Debug.Log("PlayerMovement.moveCalculations(): adjustedInput: "+adjustedInput);

            float horizSpeed = Mathf.Clamp(rb.velocity.x+adjustedInput*acceleration, -maxVelocity, maxVelocity);
            rb.velocity = new Vector2(horizSpeed, rb.velocity.y);
        
            // Update player facing information
            if (isOnGround) {
                if (motionDirection < 0) {
                    isFacingRight = false;
                } else if (motionDirection > 0) {
                    isFacingRight = true;
                }
                // zero case: leave boolean as is
            }

        }

        // ------------------------------------------------------------------------
        // Player Movement Controls
        // ------------------------------------------------------------------------

        public void EnableJump() {
            isJumpingEnabled = true;
        }

        public void DisableJump() {
            isJumpingEnabled = false;
        }

        public void EnableMoving() {
            isMovingEnabled = true;
        }

        public void DisableMoving() {
            isMovingEnabled = false;
        }

        // ------------------------------------------------------------------------
        // Player Senses
        // ------------------------------------------------------------------------
        public virtual bool isTouching(Vector2 direction, float distance) {
            RaycastHit2D hit = Physics2D.Raycast(
                transform.position, 
                direction,
                distance, 
                groundLayerMask
            );

            return hit.collider != null;
        }

        public virtual bool willTouch(Vector2 direction, float distance) {
            Vector3 futurepos = transform.position;
            if (direction == Vector2.left || direction == Vector2.right) {
                futurepos += new Vector3(raycastBuffer, 0, 0);
            } else if (direction == Vector2.up || direction == Vector2.down) {
                futurepos += new Vector3(0, raycastBuffer, 0);
            }

            RaycastHit2D hit = Physics2D.Raycast(
                futurepos, 
                direction,
                distance, 
                groundLayerMask
            );

            return hit.collider != null;
        }
    }
}


