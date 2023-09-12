using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Mirror;

public class JoinLobbyMenu : MonoBehaviour
{
    [SerializeField] private GameObject landingPagePanel = null;
    [SerializeField] private TMP_InputField addressInput = null;
    [SerializeField] private  Button joinButton = null;

    private void OnEnable()
    {
        TriviaNetworkManager.ClientOnConnected += HandleClientConnected;
        TriviaNetworkManager.ClientOnConnected += HandleClientDisconnected;

    }
    private void OnDisable()
    {
        TriviaNetworkManager.ClientOnConnected -= HandleClientConnected;
        TriviaNetworkManager.ClientOnConnected -= HandleClientDisconnected;
    }

    public void Join()
    {
        string address = addressInput.text;

        NetworkManager.singleton.networkAddress = address;
        NetworkManager.singleton.StartClient();

        joinButton.interactable = false;
    }

    private void HandleClientConnected()
    {
        joinButton.interactable = true;

        gameObject.SetActive(false);
        landingPagePanel.SetActive(false);

        Debug.LogError("Connected to: " + addressInput);
    }

    private void HandleClientDisconnected()
    {
        joinButton.interactable = true;
        Debug.LogError("Error with connection to: " + addressInput.text);
    }
}
