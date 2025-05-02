using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using Unity.Services.Lobbies;
using UnityEngine;

public class LobbyManager : MonoBehaviour
{

    public TMP_Text lobbyCodeText;
    public TMP_Text playerListText;

    private float updateInterval = 2f;
    private float timer;

    private void Start()
    {
        UpdateLobbyUI();
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= updateInterval)
        {
            timer = 0f;
            RefreshLobby();
        }
    }

    async void RefreshLobby()
    {
        if (MultiplayerManager.Instance.currentLobby != null)
        {
            Lobby lobby = await LobbyService.Instance.GetLobbyAsync(MultiplayerManager.Instance.currentLobby.Id);
            MultiplayerManager.Instance.currentLobby = lobby;
            UpdateLobbyUI();
        }
    }

    void UpdateLobbyUI()
    {
        if (MultiplayerManager.Instance.currentLobby != null)
        {
            lobbyCodeText.text = "Code: " + MultiplayerManager.Instance.currentLobby.LobbyCode;

            List<Player> players = MultiplayerManager.Instance.currentLobby.Players;
            playerListText.text = "Players:\n";
            foreach (var player in players)
            {
                playerListText.text += "- " + player.Id + "\n";
            }
        }
    }
}
