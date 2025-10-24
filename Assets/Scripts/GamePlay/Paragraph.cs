using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace GamePlay
{
    public class Paragraph : MonoBehaviour
    {
        //only 0
        public GameObject confirm;
        //only >=0
        public bool showPicture = false;//是否展示静态图
        public GameObject back;
        public int sentenceNumber;
        public int fatherSentenceNumber=-1;
        public GameObject PageCloneGameObject;
        public GameObject pagesFather;
        public List<Page> pages;
        public int pageNow = 0;

        public void Appear()
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
                print(fatherSentenceNumber);
                print(sentenceNumber);
                back.SetActive(false);
            }

        }
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

            foreach (var page in pages)
            {
                foreach (Word word in page.words)
                {
                    word.gameObject.GetComponent<AutoBox>().RefreshBox2d();
                }
            }
            Refresh();
            //只显示第一页
            for (int i = 0; i < pages.Count; i++)
            {
                print(fatherSentenceNumber);
                print(pages[i].gameObject.name);
                pages[i].gameObject.SetActive(i==0);
            }
            pageNow = 0;
            if (pageNow == pages.Count-1)
            {
                Next.SetActive(false);   
            }else
            {
                Next.SetActive(true);
            }

            if (pageNow > 0)
            {
                Last.SetActive(true);
            }
            else
            {
                Last.SetActive(false);
            }
        }

        public void Fade(float time)
        {
            foreach (Page page in pages)
            {
                page.Fade(time);
            }
        }

        public GameObject Next;
        public GameObject Last;
        public void NextPage()
        {
            pages[pageNow].gameObject.SetActive(false);
            pageNow++;
            pages[pageNow].gameObject.SetActive(true);
            if (pageNow == pages.Count-1)
            {
                Next.SetActive(false);   
            }else
            {
                Next.SetActive(true);
            }

            if (pageNow > 0)
            {
                Last.SetActive(true);
            }
            else
            {
                Last.SetActive(false);
            }
        }

        public void PreviousPage()
        {
            pages[pageNow].gameObject.SetActive(false);
            pageNow--;
            pages[pageNow].gameObject.SetActive(true);
            if (pageNow == pages.Count)
            {
                Next.SetActive(false);   
            }else
            {
                Next.SetActive(true);
            }

            if (pageNow > 0)
            {
                Last.SetActive(true);
            }
            else
            {
                Last.SetActive(false);
            }
        }
        public void ClosePanel()
        {
            foreach (Page page in pages)
            {
                page.ClosePanel();
            }
        }

        //返回上个句子
        public void Back()
        {
            gameObject.SetActive(false);
            ClosePanel();
            SentenceManager.instance.paragraphNow = fatherSentenceNumber;
            SentenceManager.instance.paragraphs[fatherSentenceNumber].gameObject.SetActive(true);
        }

        public void Refresh()
        {
            foreach (var page in pages)
            {
                page.layout.Refresh();
            }
        }
    }
}