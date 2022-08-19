using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using TMPro;
using UnityEngine.UI;

public class LobbyEntryData : MonoBehaviour
{
    // Data
    public CSteamID lobbySteamID;
    public string lobbyName;
    public TextMeshProUGUI lobbyNameText;
    public Button JoinButton;

    public void SetLobbyName()
    {
        if (lobbyName == "")
        {
            lobbyNameText.text = "Empty";
            lobbyNameText.color = Color.gray;
            JoinButton.interactable = false;
        }
        else
        {
            lobbyNameText.text = lobbyName;

            if (lobbyName.Contains("'S LOBBY"))
            {
                lobbyNameText.color = Color.yellow;
            }
            else
            {
                lobbyNameText.color = Color.gray;
                JoinButton.interactable = false;
            }
        }
    }

    public void JoinLobby()
    {
        SteamLobby.Instance.JoinLobby(lobbySteamID);
    }
}