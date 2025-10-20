using System.Collections;
using System.Collections.Generic;
using GamePlay;
using TMPro;
using UnityEngine;

public class DoubleClick1Board : MonoBehaviour
{
    public GameObject textCloneObj;
    public List<GameObject> wordList = new List<GameObject>();
    public Word word;
    public void Show(bool addChange)
    {        
        //其本身
        wordList[0].SetActive(true);
        //增添
        if (addChange)
        {
            for (int i = 1; i < wordList.Count; i++)
            {
                wordList[i].SetActive(true);
            }
        }        
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
        for (int i = 0; i < changeWordList.Count; i++)
        {
            var text = Instantiate(textCloneObj,gameObject.transform);
            text.SetActive(false);
            text.GetComponent<TMP_Text>().text = changeWordList[i];
            text.GetComponent<AutoBox>().RefreshBox2d();
            text.GetComponent<TextClick>().word = word;
            wordList.Add(text);
        }
    }
}
