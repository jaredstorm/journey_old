using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;


using Storm.Player;

namespace Storm.DialogSystem {
    public class GraphDialogManager : Singleton<GraphDialogManager> {
        public bool isInConversation;
        public bool handlingConversation;

        public float inputDelay;

        public Text speakerText;
        public Text sentenceText;

        public Animator animator;
        public Queue<Sentence> snippets;
        public Queue<Sentence> consequences;

        private GraphDialog currentGraph;

        private DialogNode currentNode;

        private Sentence currentSnippet;

        public void Start() {
            snippets  = new Queue<Sentence>();
            consequences = new Queue<Sentence>();
            if (inputDelay == 0) {
                inputDelay = 0.2f;
            }
        }



        // Begins a new dialog.
        public void StartDialog(GraphDialog dialog) {
            if (!handlingConversation) {
                _StartDialog(dialog);
            }
        }

        // Begin dialog Co-Routine.
        private void _StartDialog(GraphDialog dialog) {
            handlingConversation = true;
            isInConversation = true;

            currentGraph = dialog;
            if (currentGraph.HasStartEvents()) currentGraph.PerformStartEvents();

            SetCurrentNode(currentGraph.StartDialog());

            animator.SetBool("IsOpen", true);

            handlingConversation = false;
            NextSentence();    
        }

        private void SetCurrentNode(DialogNode node) {
            currentNode = node;
            snippets.Clear();
            foreach(Sentence s in currentNode.snippets) {
                snippets.Enqueue(s);
            }
        }

        //TODO: Add logic for decisions
        public void NextNode() {
            if (currentNode.decisions.Count > 0) {
                Decision decision = currentNode.decisions[0];
                consequences.Clear();
                foreach (Sentence s in decision.consequences) {
                    consequences.Enqueue(s);
                }
                SetCurrentNode(currentGraph.MakeDecision(decision));
            }
        }


        // Continues a dialog.
        public void NextSentence() {
            if (!handlingConversation) {
                _NextSentence();
            }
        }

        // Continue dialog Co-Routine.
        private void _NextSentence() {
            handlingConversation = true;

            if (snippets.Count == 0) {
                if (currentGraph.IsFinished()) {
                    handlingConversation = false;
                    EndDialog();
                    handlingConversation = true;

                } else {
                    // TODO: Add logic for decisions
                    
                    NextNode();

                    if (currentSnippet != null && currentSnippet.HasEvents()) {
                        currentSnippet.PerformEvents();
                    } else {
                        NextSnippet();
                    }
                    
                }

            } else {
                if (currentSnippet != null && currentSnippet.HasEvents()) {
                    currentSnippet.PerformEvents();
                } else {
                    NextSnippet();
                }
            }

            handlingConversation = false;
        }

        public bool PerformSnippetEvents() {
            if (currentSnippet.HasEvents()) {
                currentSnippet.PerformEvents();
                return true;
            }
            return false;
        }

        public void NextSnippet() {
            if (currentSnippet != null && sentenceText.text != currentSnippet.sentence) {
                StopAllCoroutines();
                sentenceText.text = currentSnippet.sentence;
                return;
            }

            if (consequences.Count > 0) {
                currentSnippet = consequences.Dequeue();
            } else {
                currentSnippet = snippets.Dequeue();
            }
        
            speakerText.text = currentSnippet.speaker;

            StopAllCoroutines();            
            StartCoroutine(_TypeSentence(currentSnippet.sentence));
        }

        IEnumerator _TypeSentence (string sentence) {
            handlingConversation = true;
            sentenceText.text = "";
            foreach(char c in sentence.ToCharArray()) {
                sentenceText.text += c;
                yield return null;
            }
            handlingConversation = false;
        }


        // Ending a dialog.
        public void EndDialog() {
            if (!handlingConversation) {
                _EndDialog();
            }
        }


        // 
        private void _EndDialog() {
            handlingConversation = true;

            animator.SetBool("IsOpen", false);

            if (currentGraph.HasCloseEvents()) currentGraph.PerformCloseEvents();

            GameObject.FindObjectOfType<PlayerCharacter>().movement.EnableMoving();

            isInConversation = false;
            handlingConversation = false;
        }
    }
}
