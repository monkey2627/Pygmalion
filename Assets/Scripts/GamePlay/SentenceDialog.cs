using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GamePlay;
using TMPro;
using UnityEngine;

public class SentenceDialog : MonoBehaviour
{
    public TW_MultiStrings_Regular tWRegular;
    public GameObject dialog;
    public TMP_Text roleName;
    public TMP_Text endText;
    public List<Word.Dialog> Dialogs;
    public int dialogLine = 0;
    public Word word;
    public static SentenceDialog Instance;
    public GameObject[] pics;
    public Dictionary<String, GameObject> picDic;
    public GameObject black;
    private void Awake()
    {
        Instance = this;
        picDic = new Dictionary<string, GameObject>();
        for (int i = 0; i < pics.Length; i++)
        {
            picDic.Add(pics[i].name, pics[i]);
        }
        gameObject.SetActive(false);
    }

    public void Show(string pic, List<Word.Dialog> dialogList, string endText,Word word)
    {
        black.GetComponent<SpriteRenderer>().DOFade(0, 0);
        gameObject.SetActive(true);
        this.word = word;
        for (int i = 0; i < pics.Length; i++)
        {
           pics[i].SetActive(false);
        }
        picDic[pic].SetActive(true);
        dialogLine = 0;
        Dialogs = dialogList;
        this.endText.DOFade(0, 0);
        this.endText.text = endText;
        Read();
    }

    public void Read()
    {
        if (Dialogs.Count > dialogLine)
        {
            roleName.text = Dialogs[dialogLine].Name;
            tWRegular.MultiStrings[0]=Dialogs[dialogLine].Text;
            tWRegular.finishOneText = false;
            tWRegular.NextString();
            dialog.SetActive(true);
            dialogLine++;
        }
        else
        {
            dialog.SetActive(false);
            if (endText.text != "")
            {
                endText.gameObject.GetComponent<AutoBox>().RefreshBox2d();
                for (int i = 0; i < pics.Length; i++)
                {
                    pics[i].SetActive(false);
                }
                endText.DOFade(1, 2);
            }
            else
            { 
                for (int i = 0; i < pics.Length; i++)
                {
                    pics[i].SetActive(false);
                }
                black.GetComponent<SpriteRenderer>().DOFade(230.0F/255.0F, 0.5f).OnComplete(() =>
                {
                    
                     gameObject.SetActive(false);
                     PygmalionGameManager.Instance.upperButtons.SetActive(true);
                     word.page.paragraph.gameObject.SetActive(true);
                    
                });
               
               
            }
        } 
    }

    public void ClickText()
    {
        dialog.SetActive(false);
        endText.DOFade(0, 0.5f).OnComplete(() => { gameObject.SetActive(false);
            word.page.paragraph.gameObject.SetActive(true);
            PygmalionGameManager.Instance.upperButtons.SetActive(true);
        });
        black.GetComponent<SpriteRenderer>().DOFade(230.0F/255.0F, 0.5f);
    }
}
