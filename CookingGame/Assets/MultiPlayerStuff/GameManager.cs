using UnityEngine;
using Unity.Netcode;
public class GameManager : NetworkBehaviour
{
    [SerializeField] private MultiplayerUI m_multiplayerUI;

   
    void Start()
    {
        if (m_multiplayerUI != null)
        {
            m_multiplayerUI.OnStartHost += StartHost;
            m_multiplayerUI.OnStartClient += StartClient;
            m_multiplayerUI.OnDisconnectClient += DisconnectClient;

        }
    }

    private void StartHost()
    {
        m_multiplayerUI.DisableButtons();
        NetworkManager.StartHost();
    }

    private void StartClient()
    {
        m_multiplayerUI.DisableButtons();
        NetworkManager.StartClient();
    }

    private void DisconnectClient()
    {
        m_multiplayerUI.DisableButtons();
        NetworkManager.Shutdown();
    }
}
