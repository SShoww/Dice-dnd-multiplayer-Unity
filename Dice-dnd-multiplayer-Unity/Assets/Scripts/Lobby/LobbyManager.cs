using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using Unity.Services.Lobbies.Models;
using Unity.Services.Lobbies;
using UnityEngine;
using System.Collections;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI codeText;
    public GameObject nameBlockPrefab;
    public Transform nameBlockParent;
    public Button backButton;
    public Button startGameButton;

    private void Start()
    {
        backButton.onClick.AddListener(LeaveLobby);
        startGameButton.onClick.AddListener(StartGame);

        // Show start button only to host
        startGameButton.gameObject.SetActive(NetworkManager.Singleton.IsHost);

        UpdateLobbyUI();
        StartCoroutine(AutoRefreshLobby());
    }

    public void UpdateLobbyUI()
    {
        // Clear previous list
        foreach (Transform child in nameBlockParent)
        {
            Destroy(child.gameObject);
        }

        // Display lobby code
        if (MultiplayerManager.Instance != null && MultiplayerManager.Instance.currentLobby != null)
        {
            codeText.text = MultiplayerManager.Instance.currentLobby.LobbyCode;

            List<Player> players = MultiplayerManager.Instance.currentLobby.Players;
            foreach (Player player in players)
            {
                string playerName = player.Data != null && player.Data.ContainsKey("playerName")
                    ? player.Data["playerName"].Value
                    : "Unknown";

                GameObject block = Instantiate(nameBlockPrefab, nameBlockParent);
                TextMeshProUGUI nameText = block.GetComponentInChildren<TextMeshProUGUI>();
                if (nameText != null)
                {
                    nameText.text = playerName;
                }
            }
        }
        else
        {
            codeText.text = "LOBBY ERROR";
        }
    }

    private void LeaveLobby()
    {
        if (MultiplayerManager.Instance != null && MultiplayerManager.Instance.currentLobby != null)
        {
            MultiplayerManager.Instance.currentLobby = null;
            SceneManager.LoadScene("MenuScene");
        }
    }

    private void StartGame()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            SceneManager.LoadScene("GameScene");
        }
    }

    private IEnumerator AutoRefreshLobby()
    {
        while (true)
        {
            yield return new WaitForSeconds(3f);
            UpdateLobbyUI();
        }
    }
}
