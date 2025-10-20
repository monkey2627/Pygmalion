using System;
using System.Collections;
using System.Collections.Generic;
using GamePlay;
using TMPro;
using UnityEngine;

public class DoubleClick2Board : MonoBehaviour
{
    public GameObject textCloneObj;
    public List<GameObject> wordList = new List<GameObject>();
    public Word word;
    public void Show(bool playedChange,bool playedDelete)
    {        
        //其本身
        wordList[0].SetActive(true);
        //替换
        if (playedChange)
        {
            for (int i = 1; i < wordList.Count-1; i++)
            {
                wordList[i].SetActive(true);
            }
        }        
        //删除
        if (playedDelete) 
            wordList[^1].SetActive(true);
                 
    }

    private void OnEnable()
    {
        for (int i = 0; i < wordList.Count; i++)
        {
            wordList[i].SetActive(false);
        }
    }

    public void Gen(List<string> changeWordList)
    {
        //其本身
        GameObject text;
        for (int i = 0; i < changeWordList.Count; i++)
        {
                text= Instantiate(textCloneObj,gameObject.transform);
                text.SetActive(false);
                text.GetComponent<TMP_Text>().text = changeWordList[i];
                text.GetComponent<AutoBox>().RefreshBox2d();
                text.GetComponent<TextClick>().word = word;
                wordList.Add(text);
        }
        //删除
        text= Instantiate(textCloneObj,gameObject.transform);
        text.SetActive(false);
        text.GetComponent<TMP_Text>().text = "/";
        text.GetComponent<AutoBox>().RefreshBox2d();
        text.GetComponent<TextClick>().word = word;
        wordList.Add(text);
    }
}
