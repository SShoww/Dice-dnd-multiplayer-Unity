using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MultiplayerManager : MonoBehaviour
{
    public static MultiplayerManager Instance;
    public string playerName;
    public Lobby currentLobby;
    public string joinCode;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private async void Start()
    {
        await InitializeUnityServices();
    }

    private async Task InitializeUnityServices()
    {
        await UnityServices.InitializeAsync();

        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }

    public async void CreateLobby()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(8);
            joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            CreateLobbyOptions options = new CreateLobbyOptions
            {
                IsPrivate = false,
                Player = new Player(id: AuthenticationService.Instance.PlayerId),
                Data = new Dictionary<string, DataObject>
                {
                    {"joinCode", new DataObject(DataObject.VisibilityOptions.Member, joinCode)}
                }
            };

            currentLobby = await LobbyService.Instance.CreateLobbyAsync("DiceRoom", 8, options);

            Debug.Log("Lobby created with code: " + currentLobby.LobbyCode);

            SetRelayServerData(allocation);
            NetworkManager.Singleton.StartHost();
            SceneManager.LoadScene("LobbyScene");
        }
        catch (Exception e)
        {
            Debug.LogError("CreateLobby failed: " + e);
        }
    }

    public async void JoinLobby(string code)
    {
        try
        {
            JoinLobbyByCodeOptions options = new JoinLobbyByCodeOptions
            {
                Player = new Player(id: AuthenticationService.Instance.PlayerId)
            };

            currentLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(code, options);
            Debug.Log("Joined lobby: " + currentLobby.LobbyCode);

            joinCode = currentLobby.Data["joinCode"].Value;
            JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            SetRelayServerData(allocation);
            NetworkManager.Singleton.StartClient();
            SceneManager.LoadScene("LobbyScene");
        }
        catch (Exception e)
        {
            Debug.LogError("JoinLobby failed: " + e);
        }
    }

    private void SetRelayServerData(Allocation allocation)
    {
        var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        transport.SetRelayServerData(
            allocation.RelayServer.IpV4,
            (ushort)allocation.RelayServer.Port,
            allocation.AllocationIdBytes,
            allocation.Key,
            allocation.ConnectionData,
            allocation.ConnectionData,
            true
        );
    }

    private void SetRelayServerData(JoinAllocation allocation)
    {
        var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        transport.SetRelayServerData(
            allocation.RelayServer.IpV4,
            (ushort)allocation.RelayServer.Port,
            allocation.AllocationIdBytes,
            allocation.Key,
            allocation.ConnectionData,
            allocation.HostConnectionData,
            true
        );
    }
}
