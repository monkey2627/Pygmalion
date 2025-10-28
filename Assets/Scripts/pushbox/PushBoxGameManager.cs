using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PushBoxGameManager : MonoBehaviour
{
    public int totalBoxs;
    public int finishedBoxs;

    public static PushBoxGameManager instance;

    private void Awake()
    {
        gameObject.SetActive(false);
        instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
            ResetStage();
    }
    public void CheckFinish()
    {
        if(finishedBoxs == totalBoxs)
        {
            print("YOU WIN!");
            SentenceManager.instance.wordClicked.ConfirmAddWord();
            //StartCoroutine(LoadNextStage());
        }
    }

    public void StartPushBoxGame()
    {
        gameObject.SetActive(true);
    }
    void ResetStage()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    IEnumerator LoadNextStage()
    {
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
