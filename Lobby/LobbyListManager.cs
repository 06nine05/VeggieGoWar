using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using System.Linq;

public class LobbyListManager : MonoBehaviour
{
    public static LobbyListManager instance;
    
    // Lobbies
    public GameObject lobbyListMenu;
    public GameObject lobbyEntryPrefab;
    public GameObject scrollViewContent;

    public GameObject lobbiesButton, hostButton;

    public List<LobbyEntryData> listOfLobbies = new List<LobbyEntryData>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void GetListOfLobbies()
    {
        lobbiesButton.SetActive(false);
        hostButton.SetActive(false);
        lobbyListMenu.SetActive(true);
        
        SteamLobby.Instance.GetListOfLobbies();
    }

    public void DisplayLobbies(List<CSteamID> lobbyIds, LobbyDataUpdate_t result)
    {
        for (int i = 0; i < lobbyIds.Count; i++)
        {
            if (lobbyIds[i].m_SteamID == result.m_ulSteamIDLobby &&
                !listOfLobbies.Any(lobby=>lobby.lobbySteamID == (CSteamID)lobbyIds[i].m_SteamID))
            {
                GameObject createdLobbyItem = Instantiate(lobbyEntryPrefab);
                
                LobbyEntryData createdLobbyDataEntry = createdLobbyItem.GetComponent<LobbyEntryData>();
                createdLobbyDataEntry.lobbySteamID = (CSteamID)lobbyIds[i].m_SteamID;
                
                createdLobbyDataEntry.lobbyName = 
                    SteamMatchmaking.GetLobbyData((CSteamID)lobbyIds[i].m_SteamID, "name");
                createdLobbyDataEntry.SetLobbyName();
                
                createdLobbyItem.transform.SetParent(scrollViewContent.transform);
                createdLobbyItem.transform.localScale = Vector3.one;
                
                listOfLobbies.Add(createdLobbyDataEntry);

                if (createdLobbyDataEntry.lobbyName.Contains("'S LOBBY"))
                {
                    createdLobbyItem.transform.SetAsFirstSibling();
                }
            }
        }
    }

    public void DestroyLobbies()
    {
        foreach (LobbyEntryData lobbyItem in listOfLobbies)
        {
            Destroy(lobbyItem.gameObject);
        }
        
        listOfLobbies.Clear();
    }
}
