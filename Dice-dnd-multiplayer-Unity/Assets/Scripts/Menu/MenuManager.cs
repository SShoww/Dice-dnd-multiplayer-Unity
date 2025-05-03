using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public TMP_InputField nameInput;
    public TMP_InputField codeInput;

    public void OnCreateRoomClicked()
    {
        string playerName = nameInput.text;
        if (string.IsNullOrEmpty(playerName)) return;

        MultiplayerManager.Instance.playerName = playerName;
        MultiplayerManager.Instance.CreateLobby();
    }

    public void OnJoinRoomClicked()
    {
        string playerName = nameInput.text;
        string roomCode = codeInput.text;

        if (string.IsNullOrEmpty(playerName) || string.IsNullOrEmpty(roomCode)) return;

        MultiplayerManager.Instance.playerName = playerName;
        MultiplayerManager.Instance.JoinLobby(roomCode);
    }
}
