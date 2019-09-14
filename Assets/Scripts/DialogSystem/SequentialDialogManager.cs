using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


using Storm.Player;

namespace Storm.DialogSystem {
    public class SequentialDialogManager : Singleton<SequentialDialogManager> {
        public bool isInConversation;
        public bool handlingConversation;

        public float inputDelay;

        public Text speakerText;

        public Text sentenceText;

        public Animator animator;
        public Queue<Sentence> snippets;


        SequentialDialog currentDialog;


        //-----------------------------------------------------------------------------------------
        // Constructors
        //-----------------------------------------------------------------------------------------
        public void Start() {
            snippets  = new Queue<Sentence>();
            if (inputDelay == 0) {
                inputDelay = 0.2f;
            }
        }


        // Begins a new dialog.
        public void StartDialog(SequentialDialog dialog) {
            if (!handlingConversation) {
                StartCoroutine(_StartDialog(dialog));
            }
        }

        // Begin dialog Co-Routine.
        private IEnumerator _StartDialog(SequentialDialog dialog) {
            handlingConversation = true;
            isInConversation = true;

            currentDialog = dialog;
            if (currentDialog.HasStartEvents() && currentDialog.startEventsDelay > 0) {
                yield return new WaitForSeconds(currentDialog.startEventsDelay);
            }

            animator.SetBool("IsOpen", true);
            snippets.Clear();
            foreach(Sentence s in dialog.snippets) {
                snippets.Enqueue(s);
            }
            handlingConversation = false;
            NextSentence();    

            yield return new WaitForSeconds(inputDelay);  
        }


        // Continues a dialog.
        public void NextSentence() {
            if (!handlingConversation) {
                StartCoroutine(_NextSentence());
            }
        }


        public void NextNode() {

        }

        // Continue dialog Co-Routine.
        private IEnumerator _NextSentence() {
            handlingConversation = true;
            if (snippets.Count == 0) {
                handlingConversation = false;
                EndDialog();
                handlingConversation = true;
            } else {
                Sentence snippet = snippets.Dequeue();
            
                speakerText.text = snippet.speaker;
                StopCoroutine("TypeSentence");
                StartCoroutine(TypeSentence(snippet.sentence));
            }
            yield return new WaitForSeconds(inputDelay);
            handlingConversation = false;
        }

        IEnumerator TypeSentence (string sentence) {
            sentenceText.text = "";
            foreach(char c in sentence.ToCharArray()) {
                sentenceText.text += c;
                yield return null;
            }
        }


        // Ending a dialog.
        public void EndDialog() {
            if (!handlingConversation) {
                StartCoroutine(_EndDialog());
            }
        }


        // 
        private IEnumerator _EndDialog() {
            handlingConversation = true;

            animator.SetBool("IsOpen", false);
            GameObject.FindObjectOfType<PlayerCharacter>().movement.EnableMoving();

            float waitTime;
            if (currentDialog.HasCloseEvents() && currentDialog.closeEventsDelay > inputDelay) {
                waitTime = currentDialog.closeEventsDelay;
            } else {
                waitTime = inputDelay;
            }

            yield return new WaitForSeconds(waitTime);

            isInConversation = false;
            handlingConversation = false;
        }
    }
}
