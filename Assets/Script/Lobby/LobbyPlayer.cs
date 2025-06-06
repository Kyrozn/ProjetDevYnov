using Mirror;
using UnityEngine;

public class LobbyPlayer : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnPlayerNameChanged))]
    public string playerName;

    public override void OnStartClient()
    {
        base.OnStartClient();
        LobbyUI.singleton.RefreshPlayerList();
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
        LobbyUI.singleton.RefreshPlayerList();
    }

    void OnPlayerNameChanged(string oldName, string newName)
    {
        // Appelé automatiquement quand le nom change
        if (isClient)
        {
            LobbyUI.singleton.RefreshPlayerList();
        }
    }

    [Command]
    public void CmdSetPlayerName(string newName)
    {
        playerName = newName;
    }
}
