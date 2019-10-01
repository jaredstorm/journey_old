using UnityEngine;
using UnityEngine.SceneManagement;

using Storm.Extensions;
using Storm.TransitionSystem;

namespace Storm.DialogSystem {
    public class CutsceneDialogManager : Singleton<CutsceneDialogManager> {

        private DialogManager manager;

        private string nextScene;

        #region Unity Functions
        //---------------------------------------------------------------------
        // Unity Functions
        //---------------------------------------------------------------------
        public override void Awake() {
            base.Awake();
            manager = GetComponent<DialogManager>();
        }

        public void Update() {
           if (manager.isInConversation && Input.GetKeyDown(KeyCode.Space)) {
                manager.NextSentence();
                if (!manager.isInConversation) {
                    Debug.Log("CHANGING!");
                    TransitionManager.Instance.MakeTransition(nextScene);
                }
            }
        }

        #endregion

        #region Dialog Handling
        //---------------------------------------------------------------------
        // Dialog Handling Functions
        //---------------------------------------------------------------------

        public void StartDialog() {
            manager.StartDialog();
        }

        #endregion

        #region Getters / Setters
        public void SetNextScene(string scene) {
            nextScene = scene;
        }

        public void SetCurrentDialog(DialogGraph dialog) {
            manager.SetCurrentDialog(dialog);
        }
        #endregion

    }
}
