using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

namespace GamePlay
{
    public enum ConfirmType
    {
        Normal,//按照所有的正确率
        OnlyOneCorrect//只有哪一个选正确了才到好结局
    }

    public class SentenceManager : MonoBehaviour
    {
        public static SentenceManager Instance;
        public ConfirmType type;
        public bool guideTime;
        public List<BackgroundWander> jellyfishs = new List<BackgroundWander>();
        public List<string> endScriptsList = new List<string>();
        public List<Sentence> sentences;
        private void Awake()
        {
            Instance = this;
        }
        
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
            
            if (guideTime)
            {
                
                GameManager.Instance.ReadLine();   
                guideTime = false;
            }
            else
            {
                switch (type)
                {
                    case ConfirmType.Normal:
                        float all = 0;
                        float right = 0;
                        foreach (var w in from s in sentences from w in s.words where w.wordType == 1||w.wordType == 2||w.wordType == 4||w.wordType == 5 select w)
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
                            GameManager.Instance.Change2ScriptAndReadLine(endScriptsList[0]);
                        }else if (ans < 0.5)
                        {
                            GameManager.Instance.Change2ScriptAndReadLine(endScriptsList[2]);
                        }
                        else
                        {
                            GameManager.Instance.Change2ScriptAndReadLine(endScriptsList[1]);
                        }
                        break;
                    case ConfirmType.OnlyOneCorrect:
                        break;
                }
            }
            
            //销毁所有的句子
            foreach (var s in sentences)
            {
                Destroy(s.gameObject);
            }
        }
        
        #region 内部数据结构
        [Serializable]
        private class Archive
        {
            public List<SentenceSnapshot> sentences = new List<SentenceSnapshot>();
        }

        [Serializable]
        private class SentenceSnapshot
        {
            public int number;
            public int fatherSentenceNumber;
            [FormerlySerializedAs("picture")] public bool showPicture;
            public List<WordSnapshot> words = new List<WordSnapshot>();
        }

        [Serializable]
        private class WordSnapshot
        {
            public int wordType;
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
            foreach (var s in SentenceManager.Instance.sentences)
            {
                var snap = new SentenceSnapshot
                {
                    number = s.scentenceNumber,
                    fatherSentenceNumber = s.fatherSentenceNumber,
                    showPicture = s.showPicture
                };

                foreach (var w in s.words)
                {
                    snap.words.Add(new WordSnapshot
                    {
                        wordType = w.wordType,
                        currentText = w.wordText.text,
                        playedDelete = w.playedDelete,
                        playedChange = w.playedChange,
                        playedAdd = w.playedAdd,
                       
                    });
                }
                archive.sentences.Add(snap);
            }
            return archive;
        }
        #endregion

        #region 恢复存档
        private void RestoreArchive(Archive data)
        {
            var mgr = SentenceManager.Instance;
            if (mgr.sentences.Count != data.sentences.Count)
            {
                Debug.LogError("存档与当前场景句子数量不一致，无法恢复！");
                return;
            }

            for (int i = 0; i < mgr.sentences.Count; i++)
            {
                var s   = mgr.sentences[i];
                var ss  = data.sentences[i];

                // 恢复 sentence 层简单字段
                s.scentenceNumber                = ss.number;
                s.fatherSentenceNumber  = ss.fatherSentenceNumber;
                s.showPicture               = ss.showPicture;

                // 恢复 words
                if (s.words.Count != ss.words.Count)
                {
                    Debug.LogError($"句子 {i} 的 word 数量不一致，跳过恢复。");
                    continue;
                }

                for (int j = 0; j < s.words.Count; j++)
                {
                    var w  = s.words[j];
                    var ws = ss.words[j];

                    w.wordText.text   = ws.currentText;
                    w.playedDelete    = ws.playedDelete;
                    w.playedChange    = ws.playedChange;
                    w.playedAdd       = ws.playedAdd;

                    // 如果后续需要根据 isRight 做分支，也可以再写逻辑
                }
            }
        }
        #endregion
    }
}