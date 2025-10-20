using System.Collections.Generic;
using UnityEngine;

namespace GamePlay
{
    public class Sentence : MonoBehaviour
    {
        public GameObject layout;
        //only 0
        public GameObject confirm;
        //only >=0
        public GameObject back;
        public List<Word> words;
        public int number;
        public string sentenceNow;
        public List<SentenceEnd> SentenceEnds = new List<SentenceEnd>();

        public void Fade(float time)
        {
            foreach (Word word in words)
            {
                word.Fade(time);
            }
        }

        public void ClosePanel()
        {
            foreach (Word word in words)
            {
                word.Close();
            }
        }
    }

    public struct SentenceEnd
    {
        public string Content;
        public int Jump2;
        //ONLY ONCE
        public bool Enable;
    }
}