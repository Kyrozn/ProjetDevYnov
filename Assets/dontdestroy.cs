using UnityEditor.UI;
using UnityEngine;

public class dontdestroy : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
}
