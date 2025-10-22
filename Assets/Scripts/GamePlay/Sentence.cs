using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace GamePlay
{
    public class Sentence : MonoBehaviour
    {
        public GameObject layout;
        //only 0
        public GameObject confirm;
        //only >=0
        public bool showPicture = false;//是否展示静态图
        public GameObject back;
        public List<Word> words;
        public int sentenceNumber;
        public int fatherSentenceNumber=-1;

        private void OnEnable()
        {
            if (fatherSentenceNumber == -1)
            {
                confirm.SetActive(true);
            }
            else if(fatherSentenceNumber>=0)
            {
                confirm.SetActive(false);
            }

            if (fatherSentenceNumber != -1 && !showPicture &&fatherSentenceNumber !=-10)
            {
                back.SetActive(true);
            }
            else if(fatherSentenceNumber==-1)
            {
                back.SetActive(false);
            }
            foreach (Word word in words)
            {
                word.gameObject.GetComponent<AutoBox>().RefreshBox2d();
            }
            layout.GetComponent<FlowLayoutGroupCentered>().Refresh();
        }

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

        //返回上个句子
        public void Back()
        {
            gameObject.SetActive(false);
            SentenceManager.Instance.sentenceNow = fatherSentenceNumber;
            SentenceManager.Instance.sentences[fatherSentenceNumber].gameObject.SetActive(true);
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