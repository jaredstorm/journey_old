using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using Storm.Player;

namespace Storm.DialogSystem {

    /*
        A collection of snippets leading up to a decision point.

        The DialogManager will run through all dialog snippets placed
        on this node and then present the user with the list of decisions.
    */
    [Serializable]
    public class SequentialDialog : MonoBehaviour {

        // The name of the dialog.
        public string key;

        // The list of dialog snippets to run through.
        public List<Sentence> snippets;


        // A sprite or object prefab to hover over the player/NPC of interest
        public GameObject indicatorPrefab;

        // The actual instance of the indicator on the screen during runtime.
        private GameObject indicatorInstance;

        // The relative position of the indicator on the
        public Vector3 indicatorPosition;

        // Events to play at the beginning of a dialog;
        public float startEventsDelay;
        public UnityEvent startEvents;


        // Events to play at the end of a dialog;
        public float closeEventsDelay;
        public UnityEvent closeEvents;


        //---------------------------------------------------------------------
        // Constructors
        //---------------------------------------------------------------------

        public SequentialDialog(string tag) {
            this.key = tag;
            snippets = new List<Sentence>();
        }

        public SequentialDialog(string tag,
                          IEnumerable<Sentence> snippets, 
                          IEnumerable<Decision> decisions) {

            this.key = tag;
            this.snippets = new List<Sentence>(snippets);
        }

        public SequentialDialog(string tag, IEnumerable<Sentence> snippets) {
            this.key = tag;
            this.snippets = new List<Sentence>(snippets);
        }



        //---------------------------------------------------------------------
        // Graph Building
        //---------------------------------------------------------------------

        public void AddSnippet(Sentence snippet) {
            snippets.Add(snippet);
        }

        public Sentence AddSnippet(string speaker, string sentence) {
            Sentence snippet = new Sentence(speaker, sentence);
            snippets.Add(snippet);
            return snippet;
        }
        public void ClearSnippets() {
            snippets.Clear();
        }


        //---------------------------------------------------------------------
        // Event Handling
        //---------------------------------------------------------------------
        public bool HasStartEvents() {
            return (startEvents.GetPersistentEventCount() > 0);
        }

        public void PerformStartEvents() {
            startEvents.Invoke();
        }

        public bool HasCloseEvents() {
            return (closeEvents.GetPersistentEventCount() > 0);
        }

        public void PerformCloseEvents() {
            closeEvents.Invoke();
        }

        //---------------------------------------------------------------------
        // Dialog Triggering
        //---------------------------------------------------------------------
        public void OnTriggerEnter2D(Collider2D other) {

            // Prepare to enter a dialog.
            if (other.CompareTag("Player")) {
                //Debug.Log("Disable Jump!");
                PlayerCharacter player = other.GetComponent<PlayerCharacter>();
                player.movement.DisableJump();

                indicatorInstance = Instantiate<GameObject>(
                    indicatorPrefab, 
                    player.transform.position+indicatorPosition, 
                    Quaternion.identity
                );
                
                indicatorInstance.transform.parent = player.transform;
            }
        }

        public void OnTriggerStay2D(Collider2D other) {
            
            // Start or continue a dialog.
            if (other.CompareTag("Player") && 
                (Input.GetKeyDown(KeyCode.Space) || Input.GetKey(KeyCode.Space))) {
                PlayerCharacter player = other.GetComponent<PlayerCharacter>();

                if (indicatorInstance != null) {
                    Destroy(indicatorInstance.gameObject);
                }

                SequentialDialogManager dialogManager = SequentialDialogManager.Instance;
                if (!dialogManager.isInConversation) {
                    //Debug.Log("Starting conversation");
                    
                    player.movement.DisableMoving();
                    dialogManager.StartDialog(this);
                } else {
                    dialogManager.NextSentence();

                }
            }
        }

        public void OnTriggerExit2D(Collider2D other) {
            if (other.CompareTag("Player")) {
                PlayerCharacter player = other.GetComponent<PlayerCharacter>();
                player.movement.EnableJump();

                if (indicatorInstance != null) {
                    Destroy(indicatorInstance.gameObject);
                }
            }
        }
    }
}

