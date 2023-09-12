using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LobbyMenu : MonoBehaviour
{
    [SerializeField] private GameObject lobbyUI = null;
    [SerializeField] private Button startGameButton = null;
    [SerializeField] private TMP_Text[] playerNameTexts = new TMP_Text[8];

    private void Start()
    {
        TriviaNetworkManager.ClientOnConnected += HandleClientConnected;
        Player.AuthorityOnPartyOwnerStateUpdated += AuthorityHandlePartyStateUpdated;
        Player.ClientOnInfoUpdated += ClientHandleInfoUpdated;
    }

    public void StartGame()
    {
        NetworkClient.connection.identity.GetComponent<Player>().CmdStartGame();
    }

    private void OnDestroy()
    {
        TriviaNetworkManager.ClientOnConnected -= HandleClientConnected;
        Player.AuthorityOnPartyOwnerStateUpdated -= AuthorityHandlePartyStateUpdated;
        Player.ClientOnInfoUpdated -= ClientHandleInfoUpdated;
    }

    private void AuthorityHandlePartyStateUpdated(bool state)
    {
        startGameButton.gameObject.SetActive(state);
    }

    private void ClientHandleInfoUpdated()
    {
        List<Player> players = ((TriviaNetworkManager)NetworkManager.singleton).Players;
        for (int i = 0; i < players.Count; i++)
        {
            playerNameTexts[i].text = players[i].GetDisplayName();
            playerNameTexts[i].color = players[i].GetColor();
        }

        for (int i = players.Count; i < playerNameTexts.Length; i++)
        {
            playerNameTexts[i].text = "Waiting For Player...";
            playerNameTexts[i].color = Color.white;
        }

        startGameButton.interactable = players.Count >= 2;
    }
    private void HandleClientConnected()
    {
        lobbyUI.SetActive(true);
    }

    public void LeaveLobby()
    {
        if(NetworkServer.active && NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopHost();
        }
        else
        {
            NetworkManager.singleton.StopClient();
        }

        SceneManager.LoadScene(0);
    }
}
