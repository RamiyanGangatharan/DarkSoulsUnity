using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DarkSouls
{
    /// <summary>
    /// Manages loading of the main world scene and stores the scene index.
    /// Implements a singleton pattern to ensure only one instance persists between scenes.
    /// </summary>
    public class WorldSaveGameManager : MonoBehaviour
    {
        // Singleton instance to ensure only one manager exists at runtime.
        public static WorldSaveGameManager instance;

        // Index of the world scene to load (configured in the inspector).
        [SerializeField] private int WorldSceneIndex = 1;

        /// <summary>
        /// Called when the script instance is being loaded.
        /// Initializes the singleton instance.
        /// </summary>
        private void Awake()
        {
            // Enforce singleton pattern.
            if (instance == null) { instance = this; }
            else { Destroy(gameObject); return; }
        }

        /// <summary>
        /// Called before the first frame update.
        /// Makes the manager persistent across scene loads.
        /// </summary>
        private void Start() { DontDestroyOnLoad(gameObject); }

        /// <summary>
        /// Loads the world scene asynchronously.
        /// </summary>
        /// <returns>Coroutine IEnumerator for async operation.</returns>
        public IEnumerator LoadNewGame()
        {
            AsyncOperation loadOperation = SceneManager.LoadSceneAsync(WorldSceneIndex);
            yield return null; // Placeholder — can be expanded to wait for loadOperation completion if needed.
        }

        /// <summary>
        /// Returns the index of the world scene.
        /// </summary>
        /// <returns>Integer index of the world scene.</returns>
        public int GetWorldSceneIndex() { return WorldSceneIndex; }
    }
}

