using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using System;

public class TriviaNetworkManager : NetworkManager
{
    public static event Action ClientOnConnected;
    public static event Action ClientOnDisconnected;

    private bool isGameInProgress = false;
    public List<Player> Players { get; } = new List<Player>();

    #region Server

    public override void OnServerConnect(NetworkConnectionToClient conn)
    {
        if (!isGameInProgress) { return; }
        conn.Disconnect();
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        Player player = conn.identity.GetComponent<Player>();

        Players.Remove(player);

        base.OnServerDisconnect(conn);
        
    }

    public override void OnStopServer()
    {
        Players.Clear();

        isGameInProgress = false;

        base.OnStopServer();
    }

    public void StartGame()
    {
        if(Players.Count < 2) { return;}

        isGameInProgress = true;

        Debug.Log("Game started!");
        //Change Scene to game
        ServerChangeScene("Game_Scene");
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);

        Player player = conn.identity.GetComponent<Player>();

        Players.Add(player);

        player.SetColor(new Color(
            UnityEngine.Random.Range(0f, 1f),
            UnityEngine.Random.Range(0f, 1f),
            UnityEngine.Random.Range(0f, 1f),
            255
            )
       );

        player.SetDisplayName($"Player {Players.Count}");


  

        player.SetPartyOwener(Players.Count == 1);
    }

    public override void OnServerChangeScene(string newSceneName)
    {
     /**   if (SceneManager.GetActiveScene().name.StartsWith(""))
        {

        }


        foreach(Player player in Players)
        {
            //Do something for each player during game start
        }
     **/
    }

    #endregion




    #region Client
    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);
        ClientOnConnected?.Invoke();
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);

        ClientOnDisconnected?.Invoke();
    }

    public override void OnStopClient()
    {
        Players.Clear();
    }

    #endregion


}
