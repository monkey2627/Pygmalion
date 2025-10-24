using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using GamePlay;
using UnityEngine;
using UnityEngine.Serialization;
    public enum ConfirmType
    {
        Normal,//按照所有的正确率
        OnlyOneCorrect//只有哪一个选正确了才到好结局
    }

    public class SentenceManager : MonoBehaviour
    {
        //存这一整个题目是从哪里开始的
        public int sentenceBeginPlace;
        public static SentenceManager instance;
        public int paragraphNow=0;
        public ConfirmType type;
        public bool guideTime;
        public List<BackgroundWander> jellyfishs = new List<BackgroundWander>();
        public List<string> endScriptsList = new List<string>();
        public List<Paragraph> paragraphs;
        public bool enAbleConfirm = false;
        public Word wordClicked;
        private void Awake()
        {
            instance = this;
            paragraphNow = 0;
        }
        public void Fade()
        {
            float time = 1;
            foreach (Paragraph sentence in paragraphs)
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

            PygmalionGameManager.Instance.isGameTime = false;
            if (guideTime)
            {
                
                PygmalionGameManager.Instance.ReadLine();   
                guideTime = false;
            }
            else
            {
                if(!enAbleConfirm) return;
                paragraphs[0].confirm.SetActive(false);
                switch (type)
                {
                    case ConfirmType.Normal:
                        float all = 0;
                        float right = 0;
                        foreach (var w in from s in paragraphs from p in s.pages from w in p.words  where w.wordType == 1||w.wordType == 2||w.wordType == 4||w.wordType == 5 select w)
                        {
                            all += 1;
                            if (w.IsRight())
                            {
                                right += 1;
                            }
                        }
                        float ans = right / all;
                        if (ans > 0.8)
                        {
                            PygmalionGameManager.Instance.Change2ScriptAndReadLine(endScriptsList[0]);
                        }else if (ans < 0.5)
                        {
                            PygmalionGameManager.Instance.Change2ScriptAndReadLine(endScriptsList[2]);
                        }
                        else
                        {
                            PygmalionGameManager.Instance.Change2ScriptAndReadLine(endScriptsList[1]);
                        }
                        break;
                    case ConfirmType.OnlyOneCorrect:
                        if (paragraphs[1].pages[0].words[2].IsRight())
                        {
                            PygmalionGameManager.Instance.Change2ScriptAndReadLine(endScriptsList[1]);
                        }
                        else
                        {
                            PygmalionGameManager.Instance.Change2ScriptAndReadLine(endScriptsList[0]);

                        }
                        break;
                }
            }
            
            //销毁所有的句子
            foreach (var s in paragraphs)
            {
                Destroy(s.gameObject);
            }
        }
        
        #region 内部数据结构
        [Serializable]
        private class Archive
        {
            public int sentenceBeginPlace;
            public int paragraphNow;
            public List<ParagraphSnapshot> paragraphs = new List<ParagraphSnapshot>();
        }

        [Serializable]
        private class ParagraphSnapshot
        {
            public int pageNow;
            public List<WordSnapshot> words = new List<WordSnapshot>();
        }
        [Serializable]
        private class WordSnapshot
        {
            public int type;
            public string currentText;          // 当前显示文本
            public bool playedDelete;
            public bool playedChange;
            public bool playedAdd;
        }
        #endregion

        #region 构建存档
        private Archive BuildArchive()
        {
            Archive archive = new Archive();
            archive.paragraphNow = paragraphNow;
            archive.sentenceBeginPlace =  sentenceBeginPlace;
            foreach (var paragraph in SentenceManager.instance.paragraphs)
            {
                var snap = new ParagraphSnapshot
                {
                    pageNow = paragraph.pageNow,
                 };
                foreach (var page in paragraph.pages)
                {
                    foreach (var w in page.words)
                    {
                        snap.words.Add(new WordSnapshot
                        {
                            type = w.wordType,
                            currentText = w.wordText.text,
                            playedDelete = w.playedDelete,
                            playedChange = w.playedChange,
                            playedAdd = w.playedAdd,
                        });
                    }

                    archive.paragraphs.Add(snap);
                }
            }
            return archive;
        }
        #endregion

        #region 恢复存档
        private void RestoreArchive(Archive data)
        {
            sentenceBeginPlace = data.sentenceBeginPlace;
            SentenceManager.instance.paragraphs = new List<Paragraph>();
            SentenceManager.instance.guideTime = false;
           // CreateSentence(sentenceBeginPlace);
            
            for (int i = 0; i < paragraphs.Count; i++)
            {
                var s   = paragraphs[i];
                var ss  = data.paragraphs[i];

                paragraphNow = data.paragraphNow;
                
                //在这里要重新生成

                foreach (var page in s.pages)
                {
                    for (int j = 0; j < page.words.Count; j++)
                    {
                        var w  = page.words[j];
                        var ws = ss.words[j];
                        w.wordType = ws.type;
                        w.wordText.text   = ws.currentText;
                        w.playedDelete    = ws.playedDelete;
                        w.playedChange    = ws.playedChange;
                        w.playedAdd       = ws.playedAdd;
                    }
                }
            }
        }
        #endregion

        public void EnableEveryWord()
        {
            foreach (var t2 in from t in paragraphs from t1 in t.pages from t2 in t1.words select t2)
            {
                t2.enable = true;
            }
        }

        public void DisableEveryWord()
        {
            foreach (var t2 in from t in paragraphs from t1 in t.pages from t2 in t1.words select t2)
            {
                t2.enable = false;
            }
        }

        public void NextPara(int nextParagraphNumber)
        {
            paragraphs[paragraphNow].gameObject.SetActive(false);
            paragraphNow = nextParagraphNumber;
            paragraphs[paragraphNow].gameObject.SetActive(true);
        }
    }