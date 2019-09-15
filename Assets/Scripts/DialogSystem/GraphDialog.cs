using System;
using System.Collections.Generic;
using System.Threading;

using UnityEngine;
using UnityEngine.Events;

using Storm.Player;

namespace Storm.DialogSystem {
    [Serializable]
    public class GraphDialog : MonoBehaviour {

        public GameObject indicatorPrefab;

        private GameObject indicatorInstance;

        public Vector3 indicatorPosition;


        // The first set of dialog in a conversation.
        private DialogNode root;

        // The current dialog node.
        private DialogNode current;

        // TODO: Make a way to import/export to XML
        public string file;

        public float startEventsDelay;
        public UnityEvent startEvents;

        

        // The GRAPH of the conversation
        // (why do developers exclusively refer to this as a tree?)
        public DialogNode[] nodes;
        
        private Dictionary<string, DialogNode> graph;

        public float closeEventsDelay;
        public UnityEvent closeEvents;

        





        //---------------------------------------------------------------------
        // Constructor(s)
        //---------------------------------------------------------------------
        public void Start() {
            if (file == "")  {
                graph = new Dictionary<string, DialogNode>();
                foreach (DialogNode n in nodes) {
                    graph.Add(n.key, n);
                }

                if (nodes.Length > 0) {
                    root = nodes[0];
                }
            }
        }





        //---------------------------------------------------------------------
        // Graph Building
        //---------------------------------------------------------------------

        public bool AddDialog(DialogNode node) {
            if (graph.Count == 0) {
                root = node;
            }
            graph[node.key] = node;
            return true;
        }

        // Add a transition from one Dialog node to another.
        public void AddTransition(string fromTag, 
                                  string optionText, 
                                  string toTag) {
                                      
            DialogNode fromNode = graph[fromTag];
            fromNode.AddDecision(optionText, toTag);
        }

        public void Clear() {
            graph.Clear();
        }




        //---------------------------------------------------------------------
        // Graph Traversal
        //---------------------------------------------------------------------

        public DialogNode GetRootNode() {
            return root;
        }

        public DialogNode StartDialog() {
            current = root;
            PerformStartEvents();
            return root;
        }

        public DialogNode MakeDecision(Decision decision) {
            current = graph[decision.destinationTag];
            return current;
        }

        public DialogNode GetCurrentDialog() {
            return current;
        }

        public bool IsFinished() {
            return current.decisions.Count == 0;
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


        private void addIndicator(PlayerCharacter player) {
            indicatorInstance = Instantiate<GameObject>(
                indicatorPrefab, 
                player.transform.position+indicatorPosition, 
                Quaternion.identity
            );

            indicatorInstance.transform.parent = player.transform;
        }

        private void removeIndicator() {
            if (indicatorInstance != null) {
                    Destroy(indicatorInstance.gameObject);
            }
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

                addIndicator(player);
            }
        }


        public void OnTriggerStay2D(Collider2D other) {
            if (root == null) return;
            
            // Start or continue a dialog.
            if (other.CompareTag("Player")) {
                PlayerCharacter player = other.GetComponent<PlayerCharacter>();
                GraphDialogManager dialogManager = GraphDialogManager.Instance;

                if (Input.GetKeyDown(KeyCode.Space)) {
                    removeIndicator();

                    if (!dialogManager.isInConversation) {    
                        player.movement.DisableMoving();
                        dialogManager.StartDialog(this);
                    } else {
                        dialogManager.NextSentence();
                    }

                } else if (indicatorInstance == null && !dialogManager.isInConversation) {
                    addIndicator(player);
                }
            }
        }

        public void OnTriggerExit2D(Collider2D other) {
            if (other.CompareTag("Player")) {
                PlayerCharacter player = other.GetComponent<PlayerCharacter>();
                player.movement.EnableJump();

                removeIndicator();
            }
        }


    }
}
