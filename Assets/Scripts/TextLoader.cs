using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class TextLoader : MonoBehaviour
{
    public TW_MultiStrings_Regular tWRegular;
    public static TextLoader Instance;
    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        
    }

    internal static Task<string> Load(string storage)
    {
        throw new NotImplementedException();
    }
    
    public void Push(string text)
    {           
        tWRegular.MultiStrings[0]=text;
        tWRegular.finishOneText = false;
        tWRegular.NextString();
    }
    /*
    public void Auto()
    {
        StartCoroutine(AutoPush());
    }
    IEnumerator AutoPush()
    {
        while (true)
        {
            if (tWRegular.finishOneText)
            {
                Debug.Log("push");
                tWRegular.finishOneText = false;
                TLSGameManager.instance.ReadLine();
            }
            yield return new WaitForSeconds(0.01f);
        }
    }*/
    
}
