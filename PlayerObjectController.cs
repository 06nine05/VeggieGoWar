using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;

public class PlayerObjectController : NetworkBehaviour
{
    //Player Data
    [SyncVar] public int ConnectionID;
    [SyncVar] public int PlayerIDNumber;
    [SyncVar(hook = nameof(SendPlayerCharacter))] 
    public int CharacterIndex;
    [SyncVar] public ulong PlayerSteamID;

    [SyncVar(hook = nameof(PlayerNameUpdate))]
    public string PlayerName;

    [SyncVar(hook = nameof(PlayerReadyUpdate))]
    public bool isReady;

    [SerializeField] private PlayerStat stat;
    
    public GameObject HealthUI;

    private CustomNetworkManager manager;

    private CustomNetworkManager Manager
    {
        get
        {
            if (manager != null)
            {
                return manager;
            }

            return manager = CustomNetworkManager.singleton as CustomNetworkManager;
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public void SendPlayerCharacter(int oldValue, int newValue)
    {
        if (isServer)
        {
            CharacterIndex = newValue;
        }

        if (isClient&&(oldValue!=newValue))
        {
            UpdateCharacter(newValue);
        }
    }

    [Command]
    public void CmdUpdatePlayerCharacter(int newValue)
    {
        SendPlayerCharacter(CharacterIndex, newValue);
    }

    private void UpdateCharacter(int index)
    {
        CharacterIndex = index;
    }

    private void PlayerReadyUpdate(bool oldValue, bool newValue)
    {
        if (isServer)
        {
            this.isReady = newValue;
        }

        if (isClient)
        {
            LobbyController.Instance.UpdatePlayerList();
        }

        if (isLocalPlayer)
        {
            CharacterSelect.Instance.OnReadyToPlay(this.isReady);
        }
    }

    public void ChangeReady()
    {
        if (hasAuthority)
        {
            CmdSetPlayerReady();
        }
    }

    [Command]
    private void CmdSetPlayerReady()
    {
        this.PlayerReadyUpdate(this.isReady, !this.isReady);
    }
    
    public override void OnStartAuthority()
    {
        CmdSetPlayerName(SteamFriends.GetPersonaName().ToString());
        gameObject.name = "LocalGamePlayer";
        LobbyController.Instance.FindLocalPlayer();
        LobbyController.Instance.UpdateLobbyName();
        
    }

    public override void OnStartClient()
    {
        Manager.GamePlayers.Add(this);
        LobbyController.Instance.UpdateLobbyName();
        LobbyController.Instance.UpdatePlayerList();
    }

    public override void OnStopClient()
    {
        Manager.GamePlayers.Remove(this);
        LobbyController.Instance.UpdatePlayerList();
    }

    [Command]
    private void CmdSetPlayerName(string PlayerName)
    {
        this.PlayerNameUpdate(this.PlayerName,PlayerName);
    }
    
    public void PlayerNameUpdate(string oldValue, string newValue)
    {
        if (isServer) //Host
        {
            this.PlayerName = newValue;
        }

        if (isClient)
        {
            LobbyController.Instance.UpdatePlayerList();
        }
    }
    
    //Start Game
    public void CanStartGame(string SceneName)
    {
        if (hasAuthority)
        {
            CmdCanStartGame(SceneName);
        }
    }

    [Command]
    public void CmdCanStartGame(string SceneName)
    {
        Manager.StartGame(SceneName);
    }

    public void CreateCharacterModel()
    {
        CharacterSelect.Instance.CreateSelectedCharacter(CharacterIndex, this.GetComponent<enableComponents>());
    }

    public PlayerStat GetStat()
    {
        return stat;
    }
}