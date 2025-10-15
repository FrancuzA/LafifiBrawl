using UnityEngine;

public class GetReady : MonoBehaviour
{
    public void StartGetReady()
    {
        BasicSpawner basicSpawner = Dependencies.Instance.GetDependancy<BasicSpawner>();
        if (basicSpawner != null && basicSpawner._runner != null)
        {
            basicSpawner.ReadyUp();
        }
        else
        {
            Debug.LogWarning("NetworkRunner not initialized yet. Please wait for the game to start.");
        }
    }
}