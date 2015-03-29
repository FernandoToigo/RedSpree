using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour
{
    void Start()
    {
        if (Application.platform == RuntimePlatform.OSXWebPlayer || Application.platform == RuntimePlatform.WindowsWebPlayer)
        {
            var button = GameObject.Find("ExitButton");
            button.SetActive(false);
        }
    }

    void Update()
    {

    }

    public void StartGame()
    {
        Application.LoadLevel("Game");
    }

    public void Exit()
    {
        Application.Quit();
    }
}