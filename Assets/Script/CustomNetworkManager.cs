
using Mirror;
using UnityEngine;

public class CustomNetworkManager : NetworkManager
{
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        Transform start = GetStartPosition();
        GameObject player = Instantiate(playerPrefab, start.position, Quaternion.identity);
        NetworkServer.AddPlayerForConnection(conn, player);
    }
}