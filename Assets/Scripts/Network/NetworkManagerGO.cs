using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Minimal Netcode for GameObjects manager. Provides start/stop helpers.
/// </summary>
public class NetworkManagerGO : MonoBehaviour
{
    public bool autoStartAsHost = false;
    public bool autoStartAsClient = false;

    void Start()
    {
        if (autoStartAsHost)
        {
            StartHost();
        }
        else if (autoStartAsClient)
        {
            StartClient();
        }
    }

    public void StartHost()
    {
        if (!NetworkManager.Singleton.IsListening)
        {
            NetworkManager.Singleton.StartHost();
        }
    }

    public void StartClient()
    {
        if (!NetworkManager.Singleton.IsListening)
        {
            NetworkManager.Singleton.StartClient();
        }
    }

    public void StartServer()
    {
        if (!NetworkManager.Singleton.IsListening)
        {
            NetworkManager.Singleton.StartServer();
        }
    }

    public void StopAll()
    {
        if (NetworkManager.Singleton.IsListening)
        {
            NetworkManager.Singleton.Shutdown();
        }
    }
}
