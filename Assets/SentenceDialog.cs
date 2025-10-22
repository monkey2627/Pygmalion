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
    public GameObject picture;
    public List<Word.Dialog> Dialogs;
    public int dialogLine = 0;
    public GameObject black;
    public static SentenceDialog Instance;

    private void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);
    }

    public void Show(string pic, List<Word.Dialog> dialogList, string endText)
    {
        picture.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(pic);
        picture.SetActive(true);
        dialog.SetActive(true);
        dialogLine = 0;
        Dialogs = dialogList;
        PygmalionGameManager.Instance.readScript = false;
        PygmalionGameManager.Instance.dialog.SetActive(true);
        this.endText.DOFade(0, 0);
        this.endText.text = endText;
        this.endText.gameObject.SetActive(false);
    }

    public void Read()
    {
        if (Dialogs.Count > dialogLine)
        {
            roleName.text = Dialogs[dialogLine].Name;
            tWRegular.MultiStrings[0]=Dialogs[dialogLine].Text;
            tWRegular.finishOneText = false;
            tWRegular.NextString();
        }
        else
        {
            dialog.SetActive(false);
            endText.gameObject.GetComponent<AutoBox>().RefreshBox2d();
            black.GetComponent<SpriteRenderer>().DOFade(1, 0f);
            endText.DOFade(1, 0);
        } 
    }

    public void ClickText()
    {
        endText.DOFade(0, 0.5f);
        black.GetComponent<SpriteRenderer>().DOFade(0, 0.5f);
    }
}
