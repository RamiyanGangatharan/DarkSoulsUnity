using UnityEngine;
using UnityEngine.SceneManagement;

namespace DarkSouls
{
    /// <summary>
    /// Handles player input using Unity's new Input System.
    /// Responsible for enabling/disabling input based on scene changes.
    /// </summary>
    public class PlayerInputManager : MonoBehaviour
    {
        // Instance for singleton pattern (ensures only one exists).
        public static PlayerInputManager instance;

        // Reference to the auto-generated input actions class.
        private PlayerControls controls;

        // Current movement input vector (x = horizontal, y = vertical).
        [SerializeField] private Vector2 movementInput;

        /// <summary>
        /// Unity Awake method is called when the script instance is being loaded.
        /// Sets up the singleton instance and subscribes to scene change events.
        /// </summary>
        private void Awake()
        {
            // Enforce singleton instance pattern.
            if (instance == null) { instance = this; }
            else { Destroy(gameObject); return; }
        }

        private void Start()
        {
            instance.enabled = false;
            SceneManager.activeSceneChanged += OnSceneChange; // Subscribe to scene change event for input based on scene.
            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// Called whenever the active scene changes.
        /// Enables player input only if the new scene is the designated world scene.
        /// </summary>
        /// <param name="oldScene">The scene being unloaded.</param>
        /// <param name="newScene">The scene being loaded.</param>
        private void OnSceneChange(Scene oldScene, Scene newScene)
        {
            instance.enabled = newScene.buildIndex == WorldSaveGameManager.instance.GetWorldSceneIndex();
        }

        /// <summary>
        /// Unity OnEnable method is called when the object becomes enabled and active.
        /// Sets up input actions and enables the movement action map.
        /// Listen to movement input; update movementInput on performed input.
        /// </summary>
        private void OnEnable()
        {
            if (controls == null)  // Initialize input controls if they haven’t been created yet.
            {
                controls = new PlayerControls();
                controls.PlayerMovement.Movement.performed += context => { movementInput = context.ReadValue<Vector2>(); };
            }
            controls.PlayerMovement.Enable();
        }

        /// <summary>
        /// Unity OnDisable method is called when the object becomes disabled or inactive.
        /// Cleans up by disabling input and unsubscribing from events.
        /// </summary>
        private void OnDisable()
        {
            SceneManager.activeSceneChanged -= OnSceneChange; // Unsubscribe from scene change event to avoid memory leaks.
            controls.PlayerMovement.Disable(); // Disable input to conserve resources.
        }
    }
}