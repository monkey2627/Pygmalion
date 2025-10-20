using System;
using System.Collections;
using System.Collections.Generic;
using GamePlay;
using UnityEngine;

public class SentenceManager : MonoBehaviour
{
    public static SentenceManager Instance;

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
    }
}
