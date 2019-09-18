using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

using TMPro;

using Storm.Extensions;
using Storm.Characters.Player;

namespace Storm.DialogSystem {
    public class DialogManager : Singleton<DialogManager> {
        public bool canStartConversation;
        public bool isInConversation;
        public bool handlingConversation;

        public TextMeshProUGUI speakerText;
        public TextMeshProUGUI sentenceText;

        public Animator animator;
        public Queue<Sentence> snippets;
        public Queue<Sentence> consequences;

        private DialogGraph currentDialog;

        private DialogNode currentDialogNode;

        private Sentence currentSnippet;

        public GameObject indicatorPrefab;

        private GameObject indicatorInstance;

        public Vector3 indicatorPosition;


        #region Unity Functions
        //---------------------------------------------------------------------
        // Unity Functions
        //---------------------------------------------------------------------
        public override void Awake() {
            base.Awake();
            DontDestroyOnLoad(GameObject.FindGameObjectWithTag("UI"));
        }


        public void Start() {
            snippets  = new Queue<Sentence>();
            consequences = new Queue<Sentence>();
        }

        public void Update() {
            if (isInConversation && Input.GetKeyDown(KeyCode.Space)) {
                NextSentence();
                if (currentDialog.IsFinished()) {
                    GameManager.Instance.player.movement.EnableJump();

                    // Prevents the player from jumping at
                    // the end of every conversation.
                    Input.ResetInputAxes();
                }
            } else if (canStartConversation && Input.GetKeyDown(KeyCode.Space)) {
                RemoveIndicator();
                GameManager.Instance.player.movement.DisableMoving();
                StartDialog(currentDialog);
            }
        }

        #endregion

        #region Dialog Handling
        //---------------------------------------------------------------------
        // Dialog Handling Functions
        //---------------------------------------------------------------------
        // Begins a new dialog.
        public void StartDialog(DialogGraph dialog) {
            if (!handlingConversation) {
                _StartDialog(dialog);
            }
        }

        // Begin dialog Co-Routine.
        private void _StartDialog(DialogGraph dialog) {
            handlingConversation = true;
            isInConversation = true;

            currentDialog = dialog;
            if (currentDialog.HasStartEvents()) currentDialog.PerformStartEvents();

            SetCurrentNode(currentDialog.StartDialog());

            animator.SetBool("IsOpen", true);

            handlingConversation = false;
            NextSentence();    
        }

        private void SetCurrentNode(DialogNode node) {
            currentDialogNode = node;
            snippets.Clear();
            foreach(Sentence s in currentDialogNode.snippets) {
                snippets.Enqueue(s);
            }
        }

        //TODO: Add logic for decisions
        public void NextNode() {
            if (currentDialogNode.decisions.Count > 0) {
                Decision decision = currentDialogNode.decisions[0];
                consequences.Clear();
                foreach (Sentence s in decision.consequences) {
                    consequences.Enqueue(s);
                }
                SetCurrentNode(currentDialog.MakeDecision(decision));
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
                if (currentDialog.IsFinished()) {
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

            if (currentDialog.HasCloseEvents()) currentDialog.PerformCloseEvents();

            GameObject.FindObjectOfType<PlayerCharacter>().movement.EnableMoving();

            isInConversation = false;
            handlingConversation = false;
        }

        #endregion

        #region Getters / Setters
        public void SetCurrentDialog(DialogGraph dialog) {
            currentDialog = dialog;
        }

        #endregion

        #region Indicator Functions

        public void AddIndicator() {
            PlayerCharacter player = GameManager.Instance.player;
            indicatorInstance = Instantiate<GameObject>(
                indicatorPrefab, 
                player.transform.position+indicatorPosition, 
                Quaternion.identity
            );

            indicatorInstance.transform.parent = player.transform;
            canStartConversation = true;
        }

        public void RemoveIndicator() {
            if (indicatorInstance != null) {
                    Destroy(indicatorInstance.gameObject);
            }
            canStartConversation = false;
        }

        #endregion
    }
}
