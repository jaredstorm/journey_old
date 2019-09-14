using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnstablePlatform : MonoBehaviour
{
    public bool canReset;

    public float resetTime;

    public float decayTime;

    public int states;

    private bool deteriorating;
    private bool resetting;

    private float decayTimer;

    private float resetTimer;

    private int curState;

    private float timeBetweenStates;

    BoxCollider2D col;

    SpriteRenderer sprite;

    private Animator anim;

    // Start is called before the first frame update
    void Start() {
        decayTimer = 0f;
        deteriorating = false;
        resetting = false;
        timeBetweenStates = decayTime/states;
        anim = GetComponent<Animator>();
        col = GetComponent<BoxCollider2D>();
        sprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update() {
        if (deteriorating) {

            decayTimer += Time.deltaTime;

            if (decayTimer > decayTime) {
                Debug.Log("Destroying!");
                resetTimer = 0;
                resetting = true;
                // "Destroy" Object
                sprite.enabled = false;
                col.enabled = false;
                deteriorating = false;
            } else if (decayTimer > curState*timeBetweenStates) {
                curState++;
                anim.SetInteger("State",curState);
            }
        } else if (canReset && resetting) {
            resetTimer += Time.deltaTime;

            if (resetTimer > resetTime) {
                // reset object!
                Debug.Log("Resetting!");
                resetting = false; 

                decayTimer = 0;
                
                sprite.enabled = true;
                col.enabled = true;

                curState = 0;
                anim.SetInteger("State",curState);
            }
        }


    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) deteriorating = true;
    }
}
