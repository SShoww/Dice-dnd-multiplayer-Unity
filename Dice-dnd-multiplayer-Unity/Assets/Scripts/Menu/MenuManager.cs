using TMPro;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public TMP_InputField nameInputField;
    public TMP_InputField codeInputField;

    public void OnClickCreateRoom()
    {
        MultiplayerManager.Instance.playerName = nameInputField.text;
        MultiplayerManager.Instance.CreateLobby();
    }

    public void OnClickJoinRoom()
    {
        MultiplayerManager.Instance.playerName = nameInputField.text;
        string joinCode = codeInputField.text;
        MultiplayerManager.Instance.JoinLobby(joinCode);
    }
}
