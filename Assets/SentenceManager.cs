using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GamePlay;
using UnityEngine;

public class SentenceManager : MonoBehaviour
{
    public static SentenceManager Instance;
    public List<BackgroundWander> jellyfishs = new List<BackgroundWander>();
    private void Awake()
    {
        Instance = this;
    }

    public List<Sentence> sentences;

    public void Fade()
    {
        float time = 1;
        foreach (Sentence sentence in sentences)
        {
            sentence.Fade(time);
        }

        foreach (BackgroundWander jellyfish in jellyfishs)
        {
            jellyfish.gameObject.GetComponent<SpriteRenderer>().DOFade(0,time);
            jellyfish.Text.DOFade(0,time);
        }
    }

    //根据单词的正确比例来载入对应的结局
    public void Confirm()
    {
        
    }
}
