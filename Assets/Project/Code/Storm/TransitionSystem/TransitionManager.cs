using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Storm.Extensions;
using Storm.Characters.Player;

namespace Storm.TransitionSystem {
        
    /*
        This class handles transitioning the player between scenes.
        When a scene loads, each instance of a Transition adds its 
        tag and the location the player will be placed following the
        transition.
    */
    public class TransitionManager : Singleton<TransitionManager> {

        public Animator transitionAnim;

        private Dictionary<string, Vector3> spawnPoints = new Dictionary<string, Vector3>();
        private Dictionary<string, bool> spawnLeftRight = new Dictionary<string, bool>();
        private string currentSpawn;
        private string currentScene;

        public override void Awake() {
            base.Awake();
        }

        public void setCurrentSpawn(string spawnName)
        {
            currentSpawn = spawnName;
        }

        public void setCurrentScene(string sceneName)
        {
            currentScene = sceneName;
        }

        public void RegisterSpawn(string tag, Vector3 pos, bool right)
        {
            if (tag == null) return;
            if (!spawnPoints.ContainsKey(tag))
            {
                spawnPoints.Add(tag, pos);
                spawnLeftRight.Add(tag, right);
            }
        }

        public void Clear()
        {
            spawnPoints.Clear();
            spawnLeftRight.Clear();
        }

        public void MakeTransition(string scene) {
            MakeTransition(scene, "");
        }

        public void MakeTransition(string scene, string spawn)
        {
            Clear();
            transitionAnim.SetBool("FadeToBlack", true);
            currentSpawn = spawn;
            currentScene = scene;
        }

        /* Animation event callback. Called after the animation triggered in MakeTransition finishes. */
        public void OnTransitionComplete() {
            transitionAnim.SetBool("FadeToBlack", false);
            SceneManager.LoadScene(currentScene);
        }

        public void ReloadScene()
        {
            SceneManager.LoadScene(currentScene);
        }

        public Vector3 getSpawnPosition()
        {
            if (!spawnPoints.ContainsKey(currentSpawn)) {
                PlayerCharacter player = GameManager.Instance.player;
                spawnPoints.Add("SCENE_START", player.transform.position);
            }

            return spawnPoints[currentSpawn];
        }

        public bool getSpawningRight()
        {
            return spawnLeftRight[currentSpawn];
        }
    }

}