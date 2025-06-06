using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class CustomNetworkManager : NetworkManager
{
    public static new CustomNetworkManager singleton;

    public List<LobbyPlayer> lobbyPlayers = new();

    public override void Awake()
    {
        base.Awake();
        singleton = this;
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        GameObject player = Instantiate(playerPrefab);
        NetworkServer.AddPlayerForConnection(conn, player);

        LobbyPlayer lobbyPlayer = player.GetComponent<LobbyPlayer>();
        lobbyPlayers.Add(lobbyPlayer);
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        if (conn.identity != null)
        {
            LobbyPlayer player = conn.identity.GetComponent<LobbyPlayer>();
            lobbyPlayers.Remove(player);
        }

        base.OnServerDisconnect(conn);
    }

    public void StartGame()
    {
        if (NetworkServer.active)
        {
            ServerChangeScene("Game");
        }
    }
}
