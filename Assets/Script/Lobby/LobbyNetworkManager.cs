using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;



/*

Dans la scène du lobby, lorsque le joueur clique sur son perso :

CustomNetworkManager.singleton.SetPlayerCharacter(NetworkClient.connection, index);
Et dans l’inspecteur Unity : renseigne tous les prefabs joueurs dans playerPrefabs (dans l’ordre : Guerrier = 0, Mage = 1, etc.).

*/
public class CustomNetworkManager : NetworkManager
{
    public static new CustomNetworkManager singleton;

    public List<LobbyPlayer> lobbyPlayers = new();
    public List<GameObject> playerPrefabs; // Liste des prefabs de personnages
    private readonly Dictionary<NetworkConnectionToClient, int> selectedCharacters = new();

    public override void Awake()
    {
        base.Awake();
        singleton = this;
        DontDestroyOnLoad(this.gameObject);
    }

    public void SetPlayerCharacter(NetworkConnectionToClient conn)
    {
        selectedCharacters[conn] = PlayerPrefs.GetInt("indexChar");
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        SetPlayerCharacter(conn);
        int index = 0;
        if (selectedCharacters.TryGetValue(conn, out int selectedIndex))
        {
            index = selectedIndex;
        }

        GameObject player = Instantiate(playerPrefabs[index], GetStartPosition().position, Quaternion.identity);
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
        if (PlayerPrefs.HasKey("port"))
        {
            ServerChangeScene("Game");
        }
    }
}