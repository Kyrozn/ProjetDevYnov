using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class loader : MonoBehaviour
{
    public bool Isconnected = false;
    public GameObject Bar;
    public GameObject FormConnect;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        FormConnect.SetActive(false);
        RectTransform rt = Bar.GetComponent<RectTransform>();
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
        RectTransform rt = Bar.GetComponent<RectTransform>();

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
