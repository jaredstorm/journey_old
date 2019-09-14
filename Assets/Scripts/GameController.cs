using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Storm.Player;

public class GameController : Singleton<GameController>
{
    public TransitionManager transitions;

    public SpawnPoint initialSpawn;

    public float gravity;
    public PlayerCharacter playerPrefab;

    private Animator UIAnimator;

    protected GameController() {}

    // Start is called before the first frame update
    void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(this);
        } else {
            DontDestroyOnLoad(this);
        }

        Physics2D.gravity = new Vector2(0,-gravity);
        transitions = new TransitionManager();

        if (initialSpawn == null) {
            transitions.RegisterSpawn("SCENE_START", GameObject.FindGameObjectWithTag("Player").transform.position, true);
            transitions.setCurrentSpawn("SCENE_START");
        } else {
            transitions.setCurrentSpawn(initialSpawn.spawnName);
        }
        transitions.setCurrentScene(SceneManager.GetActiveScene().name);

    }

    void Start() {
        var cam = FindObjectOfType<TargettingCamera>();
        var player = FindObjectOfType<PlayerCharacter>();
        RespawnPlayer(player);
        
        cam.transform.position = player.transform.position;

        UIAnimator = GetComponent<Animator>();
    }


    public void KillPlayer(PlayerCharacter player) {

        //UIAnimator.SetBool("Faded", true);  
        RespawnPlayer(player);
    }

    IEnumerator _RespawnPlayer (PlayerCharacter player) {
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

/*
    This class handles transitioning the player between scenes.
    When a scene loads, each instance of a Transition adds its 
    tag and the location the player will be placed following the
    transition.
 */
public class TransitionManager {
    private Dictionary<string, Vector3> spawnPoints = new Dictionary<string, Vector3>();
    private Dictionary<string, bool> spawnLeftRight = new Dictionary<string, bool>();
    private string currentSpawn;
    private string currentScene;

    public void setCurrentSpawn(string spawnName) {
        currentSpawn = spawnName;
    }

    public void setCurrentScene(string sceneName) {
        currentScene = sceneName;
    }

    public void RegisterSpawn(string tag, Vector3 pos, bool right) {
        if (tag == null) return;
        if (!spawnPoints.ContainsKey(tag)) {
            spawnPoints.Add(tag, pos);
            spawnLeftRight.Add(tag, right);
        }
    }

    public void Clear() {
        spawnPoints.Clear();
        spawnLeftRight.Clear();
    }

    public void MakeTransition(string scene, string spawn) {
        Clear();
        SceneManager.LoadScene(scene);
        currentSpawn = spawn;
        currentScene = scene;
    }

    public void ReloadScene() {
        SceneManager.LoadScene(currentScene);
    }

    public Vector3 getSpawnPosition() {
        return spawnPoints[currentSpawn];
    }

    public bool getSpawningRight() {
        return spawnLeftRight[currentSpawn];
    }
}
