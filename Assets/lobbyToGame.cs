using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class LobbyToGame : MonoBehaviour
{
    public void StartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
    }
}
