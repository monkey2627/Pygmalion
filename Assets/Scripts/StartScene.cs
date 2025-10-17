using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class StartScene : MonoBehaviour
{
    public static StartScene Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void StartNewGame()
    {
        gameObject.SetActive(false);
        GameManager.Instance.StartNewGame();
    }
}
