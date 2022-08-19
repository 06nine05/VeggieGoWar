using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;
using TMPro;

public class SteamLobby : MonoBehaviour
{
    public static SteamLobby Instance;
    
    //Callbacks
    protected Callback<LobbyCreated_t> LobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> JoinRequest;
    protected Callback<LobbyEnter_t> LobbyEntered;
    
    //Lobby List Callbacks
    protected Callback<LobbyMatchList_t> LobbyList;
    protected Callback<LobbyDataUpdate_t> LobbyListUpdated;

    private List<CSteamID> lobbyIDs = new List<CSteamID>();

    //Variables
    public ulong CurrentLobbyID;
    private const string HostAddressKey = "HostAddress";
    private CustomNetworkManager manager;
    private GameObject notConnectedSteamPanel;
    
    //Gameobject
    public GameObject HostButton;
    public TextMeshProUGUI LobbyNameText;

    public void Start()
    {
        if (!SteamManager.Initialized)
        {
            notConnectedSteamPanel.SetActive(true);
            return;
        }

        if (Instance == null)
        {
            Instance = this;
        }
        
        manager = GetComponent<CustomNetworkManager>();

        LobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        JoinRequest = Callback<GameLobbyJoinRequested_t>.Create(OnJoinRequest);
        LobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);

        LobbyList = Callback<LobbyMatchList_t>.Create(OnGetLobbyList);
        LobbyListUpdated = Callback<LobbyDataUpdate_t>.Create(OnGetLobbyData);
    }
    
    public void HostLobby()
    {
        //SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, manager.maxConnections);
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, manager.maxConnections);
    }
    
    private void OnDestroy()
    {
        if (SteamManager.Initialized)
            SteamMatchmaking.LeaveLobby((CSteamID)CurrentLobbyID);
        LobbyCreated.Unregister();
        JoinRequest.Unregister();
        LobbyEntered.Unregister();
        LobbyList.Unregister();
        LobbyListUpdated.Unregister(); 
    }

    public void OnExit()
    {
        Application.Quit();
    }

    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        if (callback.m_eResult != EResult.k_EResultOK)
        {
            return;
        }

        Debug.Log("Lobby created Successfully");
        //if (!this) { return; }
        
        manager.StartHost();

        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey
            , SteamUser.GetSteamID().ToString());
        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "name"
            , SteamFriends.GetPersonaName().ToString() + "'S LOBBY");
    }

    private void OnJoinRequest(GameLobbyJoinRequested_t callback)
    {
        Debug.Log("Request To Jon Lobby");
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    private void OnLobbyEntered(LobbyEnter_t callback)
    {
        //Everyone
        //HostButton.SetActive(false);
        CurrentLobbyID = callback.m_ulSteamIDLobby;
        //LobbyNameText.gameObject.SetActive(true);
        //LobbyNameText.text = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "name");
        
        //Client
        if (NetworkServer.active)
        {
            return;
        }
        
        manager.networkAddress = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey);
        //if (!this) { return; }

        if (!string.IsNullOrWhiteSpace(manager.networkAddress))
        {
            manager.StartClient();
        }
    }

    public void JoinLobby(CSteamID lobbyID)
    {
        SteamMatchmaking.JoinLobby(lobbyID);
    }

    public void GetListOfLobbies()
    {
        if (lobbyIDs.Count > 0)
        {
            lobbyIDs.Clear();
        }
        
        SteamMatchmaking.AddRequestLobbyListResultCountFilter(50);
        SteamAPICall_t try_getList = SteamMatchmaking.RequestLobbyList();
    }

    void OnGetLobbyData(LobbyDataUpdate_t result)
    {
        LobbyListManager.instance.DisplayLobbies(lobbyIDs, result);
    }

    void OnGetLobbyList(LobbyMatchList_t result)
    {
        if (LobbyListManager.instance.listOfLobbies.Count > 0)
        {
            LobbyListManager.instance.DestroyLobbies();
        }

        for (int i = 0; i < result.m_nLobbiesMatching; i++)
        {
            CSteamID lobbyID = SteamMatchmaking.GetLobbyByIndex(i);
            lobbyIDs.Add(lobbyID);
            SteamMatchmaking.RequestLobbyData(lobbyID);
        }
    }
}
