using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Storm.Menus {
    public class MainMenu : MonoBehaviour {
        public void PlayGame() {
            SceneManager.LoadScene("LiveWire");
        }

        public void QuitGame() {
            Debug.Log("Quitting!");
            Application.Quit();
        }
    }
}
