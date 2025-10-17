using System.Collections.Generic;
using UnityEngine;

namespace GamePlay
{
    public class Sentence : MonoBehaviour
    {
        public List<Word> words;
        public int number;
        public string sentenceNow;
        public List<SentenceEnd> SentenceEnds = new List<SentenceEnd>();
   
    }

    public struct SentenceEnd
    {
        public string Content;
        public int Jump2;
        //ONLY ONCE
        public bool Enable;
    }
}