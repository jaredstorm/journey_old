using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Storm.Extensions;
using Storm.Characters.Player;
using Storm.Cameras;
using Storm.TransitionSystem;
using Storm.ResetSystem;
using Storm.AudioSystem;
using Storm.DialogSystem;

namespace Storm {

    public class GameManager : Singleton<GameManager> {
        public TransitionManager transitions;
        public ResetManager resets;
        public AudioManager sounds;
        public DialogManager dialogs;

        public SpawnPoint initialSpawn;

        public float gravity;

        public PlayerCharacter player;

        private Animator UIAnimator;

        // Start is called before the first frame update
        public override void Awake() {
            base.Awake();
            transitions = TransitionManager.Instance;
            resets = ResetManager.Instance;
            sounds = AudioManager.Instance;
            dialogs = DialogManager.Instance;

            Physics2D.gravity = new Vector2(0, -gravity);
            
            if (initialSpawn == null) {
                transitions.RegisterSpawn("SCENE_START", GameObject.FindGameObjectWithTag("Player").transform.position, true);
                transitions.setCurrentSpawn("SCENE_START");
            }
            else {
                transitions.setCurrentSpawn(initialSpawn.name);
            }
            transitions.setCurrentScene(SceneManager.GetActiveScene().name);

        }

        void Start() {
            var cam = FindObjectOfType<TargettingCamera>();
            player = FindObjectOfType<PlayerCharacter>();
            RespawnPlayer(player);

            cam.transform.position = player.transform.position;

            UIAnimator = GetComponent<Animator>();
        }


        public void FixedUpdate() {
            if (player == null) {
                player = FindObjectOfType<PlayerCharacter>();
            }
        }

        public void KillPlayer(PlayerCharacter player) {
            resets.Reset();
            RespawnPlayer(player);
        }

        IEnumerator _RespawnPlayer(PlayerCharacter player) {
            yield return new WaitForSeconds(1.5f);
            player.transform.position = transitions.getSpawnPosition();
            UIAnimator.SetBool("Faded", false);
        }

        public void RespawnPlayer(PlayerCharacter player) {
            //TODO: Add spawn particles
            player.transform.position = transitions.getSpawnPosition();
            if (player.rb != null) {
                player.rb.velocity = Vector3.zero;
            }
        }
    }
}