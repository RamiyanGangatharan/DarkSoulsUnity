using Unity.Netcode;
using UnityEngine;

namespace DarkSouls
{
    /// <summary>
    /// Manages the player's UI and handles network connection transitions (host to client).
    /// Implements a singleton to persist across scenes.
    /// </summary>
    public class PlayerUIManager : MonoBehaviour
    {
        public static PlayerUIManager instance;

        [Header("NETWORK JOIN")]
        [SerializeField] private bool startGameAsClient;

        /// <summary>
        /// Called when the script instance is being loaded.
        /// Initializes the singleton instance.
        /// </summary>
        private void Awake()
        {
            if (instance == null) { instance = this; }
            else { Destroy(gameObject); return; }
        }

        /// <summary>
        /// Makes the UI manager persist across scene loads.
        /// </summary>
        private void Start() { DontDestroyOnLoad(gameObject); }

        /// <summary>
        /// Checks for client startup trigger.
        /// If flagged, restarts the network manager as a client.
        /// </summary>
        private void Update()
        {
            // NOTE: This current implementation resets and restarts the client every frame.
            if (startGameAsClient)
            {
                startGameAsClient = false;
                NetworkManager.Singleton.Shutdown(); // We must first shut down because we were running as host in the title screen.
                NetworkManager.Singleton.StartClient(); // Then restart as a client.
            }
        }
    }
}
