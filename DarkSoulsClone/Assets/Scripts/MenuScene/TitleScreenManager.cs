using Unity.Netcode;
using UnityEngine;

namespace DarkSouls
{
    /// <summary>
    /// Manages interactions on the title screen, such as starting the game as a host or loading a new game.
    /// </summary>
    public class TitleScreenManager : MonoBehaviour
    {
        /// <summary>
        /// Starts the game as a host using Unity Netcode.
        /// Called by a UI button on the title screen.
        /// </summary>
        public void StartNetworkAsHost() { NetworkManager.Singleton.StartHost(); }

        /// <summary>
        /// Begins loading the world scene asynchronously.
        /// Called when the player chooses to start a new game.
        /// </summary>
        /// 
        public void StartNewGame() { StartCoroutine(WorldSaveGameManager.instance.LoadNewGame()); }
    }

}
