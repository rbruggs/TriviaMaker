using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class Player : NetworkBehaviour
{
    [SyncVar(hook = nameof(AuthorityHandlePartyOwnerStateUpdated))]
    private bool isPartyOwner =false;

    [SyncVar(hook = nameof(ClientHandleDisplayNameUpdated))]
    private string displayName;

    [SyncVar(hook = nameof(ClientHandleDisplayColorUpdated))]
    private Color playerColor;

    public static event Action<bool> AuthorityOnPartyOwnerStateUpdated;
    public static event Action ClientOnInfoUpdated;

    public Color GetColor()
    {
        return playerColor;
    }

    public bool GetIsPartyOwner()
    {
        return isPartyOwner;
    }

    public string GetDisplayName()
    {
        return displayName;
    }

    [Server]
    public void SetColor(Color newColor)
    {
        playerColor = newColor;
    }

    [Server]
    public void SetPartyOwener(bool state)
    {
        isPartyOwner = state;
    }

    [Server]
    public void SetDisplayName(string newName)
    {
        this.displayName = newName;
    }


    public override void OnStartServer()
    {
        DontDestroyOnLoad(gameObject);
    }

    public override void OnStartClient()
    {
        if (NetworkServer.active) { return; }

        DontDestroyOnLoad(gameObject);

        ((TriviaNetworkManager)NetworkManager.singleton).Players.Add(this);
    }

    public override void OnStopClient()
    {
        ClientOnInfoUpdated?.Invoke();

        if (!isClientOnly) { return; }

        ((TriviaNetworkManager)NetworkManager.singleton).Players.Remove(this);

        if (!hasAuthority) { return; }

    }

    [Command]
    public void CmdStartGame()
    {
        if (!isPartyOwner) { return; }

        ((TriviaNetworkManager)NetworkManager.singleton).StartGame();
    }


    private void AuthorityHandlePartyOwnerStateUpdated(bool oldState, bool newState)
    {
        if (!hasAuthority) { return; }

        AuthorityOnPartyOwnerStateUpdated?.Invoke(newState);
    }

    private void ClientHandleDisplayNameUpdated(string oldDisplayName, string newDisplayName)
    {
        ClientOnInfoUpdated?.Invoke();
    }
    private void ClientHandleDisplayColorUpdated(Color oldColor, Color newColor)
    {
        ClientOnInfoUpdated?.Invoke();
    }
}
