using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MatchmakingLoading : MonoBehaviour
{
    public Sprite[] listSprite;
    public GameObject logo;
    void Awake()
    {
        logo.GetComponent<Image>().sprite = listSprite[Random.Range(0, listSprite.Length)];
        logo.GetComponent<Image>().SetNativeSize();
        GameObject.Find("LeftMatchmakingButton").GetComponent<Button>().onClick.AddListener(() => LeftMatchmaking());
    }
    public void LeftMatchmaking()
    {
        SceneManager.LoadScene("Lobby");
    }
}
