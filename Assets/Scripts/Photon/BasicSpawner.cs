using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BasicSpawner : MonoBehaviour, INetworkRunnerCallbacks
{
    public NetworkRunner _runner;
    private GamePhaseManager gamePhaseManager;
    private bool isJoining;

    public void Start()
    {
        Dependencies.Instance.UnregisterDependency<BasicSpawner>();
        Dependencies.Instance.RegisterDependency<BasicSpawner>(this);
        DontDestroyOnLoad(this.gameObject);
    }

    async void StartGame(GameMode mode)
    {
        // Prevent multiple concurrent join attempts
        if (isJoining)
        {
            return;
        }
        isJoining = true;

        try
        {
            // Check if a runner exists and destroy it before creating a new one
            if (_runner != null)
            {
                // Shutdown the existing runner
                await _runner.Shutdown();
                Destroy(_runner.gameObject);
                _runner = null;
            }

            // Create a completely new GameObject for the NetworkRunner
            GameObject runnerGameObject = new GameObject("Network Runner");
            _runner = runnerGameObject.AddComponent<NetworkRunner>();
            _runner.ProvideInput = true;

            // Get the build index for TestingScene
            int sceneIndex = SceneUtility.GetBuildIndexByScenePath("TestingScene");
            var scene = SceneRef.FromIndex(sceneIndex);
            var sceneInfo = new NetworkSceneInfo();
            if (scene.IsValid)
            {
                sceneInfo.AddSceneRef(scene, LoadSceneMode.Additive);
            }

            // Start or join (depends on gamemode) a session with a specific name
            await _runner.StartGame(new StartGameArgs()
            {
                GameMode = mode,
                SessionName = "TestRoom",
                Scene = scene, // Use the TestingScene
                SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
            });

            // Only spawn the GamePhaseManager if we're the server/host
            if (_runner.IsServer || _runner.IsSharedModeMasterClient)
            {
                //var prefab = Resources.Load<NetworkObject>("GamePhaseManager");
                //Debug.Log(prefab);
                //if (prefab != null)
                
                    var networkObject = _runner.Spawn(Resources.Load<NetworkObject>("GamePhaseManager"));
                    gamePhaseManager = networkObject.GetComponent<GamePhaseManager>();
               
                
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to start game: {e.Message}");
            // Clean up on error
            if (_runner != null)
            {
                Destroy(_runner.gameObject);
                _runner = null;
            }
        }
        finally
        {
            isJoining = false;
        }
    }

    public void CreateGame()
    {
        if (_runner == null && !isJoining)
        {
            // Only start the game, let Fusion handle the scene loading
            StartGame(GameMode.Host);
            // Removed SceneManager.LoadScene("TestingScene");
        }
    }

    public void JoinGame()
    {
        if (_runner == null && !isJoining)
        {
            StartGame(GameMode.Client);
            // Removed SceneManager.LoadScene("TestingScene");
        }
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
        Debug.Log($"Disconnected from server: {reason}");
        CleanupNetworkRunner();
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        Debug.Log($"Runner shutdown: {shutdownReason}");
        CleanupNetworkRunner();
    }

    private void CleanupNetworkRunner()
    {
        if (_runner != null)
        {
            Destroy(_runner.gameObject);
            _runner = null;
        }
        isJoining = false;
        gamePhaseManager = null;
    }

    public void ReadyUp()
    {
        // Make sure we have a valid runner
        if (_runner == null)
        {
            Debug.LogError("Cannot ready up - NetworkRunner is not initialized!");
            return;
        }

        // Try to find GamePhaseManager if we don't have it
        if (gamePhaseManager == null)
        {
            // Find all GamePhaseManager instances in the scene
            var managers = FindObjectsOfType<GamePhaseManager>();
            foreach (var manager in managers)
            {
                // Only use the one that's properly networked
                if (manager.Object != null && manager.Object.IsValid)
                {
                    gamePhaseManager = manager;
                    break;
                }
            }

            if (gamePhaseManager == null)
            {
                Debug.LogError("Cannot ready up - GamePhaseManager not found or not properly initialized!");
                return;
            }
        }

        // Verify the GamePhaseManager is properly initialized
        if (!gamePhaseManager.Object.IsValid)
        {
            Debug.LogError("Cannot ready up - GamePhaseManager is not properly initialized!");
            return;
        }

        // Get your units from backpack and send to manager
        SendUnitsToManager();

        // Notify you're ready
        gamePhaseManager.RPC_PlayerReady();
    }

    private void SendUnitsToManager()
    {
        // This is where you get your unit data from your backpack system
        // For now, I'll show a placeholder implementation
        /*
        var backpack = FindObjectOfType<BackpackSystem>(); // You'll need to create this
        List<UnitData> myUnits = backpack.GetPlacedUnits();

        // Determine if we're player 1 or 2 and send units accordingly
        int playerId = _runner.LocalPlayer.PlayerId;

        // You'll need to add RPC methods to GamePhaseManager to receive units
        gamePhaseManager.RPC_ReceiveUnits(myUnits.ToArray(), playerId);
        */
    }

    // Implement other INetworkRunnerCallbacks methods
    public void OnConnectedToServer(NetworkRunner runner) 
    { 
        Debug.Log("Connected to server successfully");
    }
    
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) 
    {
        // If we're a client, try to find the GamePhaseManager
        if (!runner.IsServer && !runner.IsSharedModeMasterClient)
        {
            StartCoroutine(FindGamePhaseManagerWithRetry());
        }
    }

    private System.Collections.IEnumerator FindGamePhaseManagerWithRetry()
    {
        float timeout = 5f; // 5 seconds timeout
        float elapsed = 0f;

        while (gamePhaseManager == null && elapsed < timeout)
        {
            var managers = FindObjectsOfType<GamePhaseManager>();
            foreach (var manager in managers)
            {
                if (manager.Object != null && manager.Object.IsValid)
                {
                    gamePhaseManager = manager;
                    break;
                }
            }

            if (gamePhaseManager == null)
            {
                elapsed += 0.1f;
                yield return new WaitForSeconds(0.1f);
            }
        }

        if (gamePhaseManager == null)
        {
            Debug.LogWarning("Could not find valid GamePhaseManager after " + timeout + " seconds");
        }
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnInput(NetworkRunner runner, NetworkInput input) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
}
