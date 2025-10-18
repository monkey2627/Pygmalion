using System.Collections;
using System.Collections.Generic;
using GamePlay;
using TMPro;
using UnityEngine;

public class DoubleClick2Board : MonoBehaviour
{
    public GameObject textCloneObj;
    public Word word;
    public void Show(List<string> wordList)
    {
        foreach (string aword in wordList)
        {
            GameObject text= Instantiate(textCloneObj,gameObject.transform);
            text.GetComponent<TMP_Text>().text = aword;
            text.GetComponent<AutoBox>().RefreshBox2d();
            text.GetComponent<TextClick>().word = word;
        }
        
    }
}
