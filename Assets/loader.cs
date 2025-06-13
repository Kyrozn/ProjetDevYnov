using UnityEngine;

public class Loader : MonoBehaviour
{
    public bool Isconnected = false;
    public GameObject Bar;
    public GameObject FormConnect;
    private RectTransform rt;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        FormConnect.SetActive(false);
        rt = Bar.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(0.5f, rt.sizeDelta.y);
        if (PlayerPrefs.HasKey("id"))
        {
            rt.sizeDelta = new Vector2(90f, rt.sizeDelta.y);
            Isconnected = true;
        }
        else
        {
            FormConnect.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (rt.sizeDelta.x <= 79f)
        {
            float randomWidth = UnityEngine.Random.Range(1f, 20f);
            rt.sizeDelta = new Vector2(rt.sizeDelta.x + randomWidth, rt.sizeDelta.y);
        }
        else if (Isconnected == true)
        {
            rt.sizeDelta = new Vector2(100f, rt.sizeDelta.y);
        }
        if (rt.sizeDelta.x >= 100f)
        {
            if (Isconnected == true)
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("Lobby");
            }
        }
    }
}
