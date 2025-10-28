using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ani;
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

        public GameObject delay;
        //根据单词的正确比例来载入对应的结局
        public void Confirm()
        {
            PygmalionGameManager.Instance.isGameTime = false;
          
            if(!enAbleConfirm) return;
            Fade();
            paragraphs[0].confirm.SetActive(false);
            WaterAni.Instance.FadeJellyFishs();
            WaterAni.Instance.FadeTarget();
            delay.transform.DOMove(new Vector3(0, 0, 0), 2).OnComplete(() => { 
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
            //销毁所有的句子
            foreach (var s in paragraphs)
            {
                Destroy(s.gameObject);
            }});
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
            public bool enable;
            public bool hasRun;
            public bool hasSpecial2NextPara;
            public string currentText;          // 当前显示文本
            public bool playedDelete;
            public bool playedChange;
            public bool playedAdd;
        }
        #endregion

        #region 构建存档
        public void Save(int type=0)
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
                            hasRun = w.hasRun,
                            enable = w.enable,
                            hasSpecial2NextPara = w.hasSpecial2NextPara,
                            currentText = w.wordText.text,
                            playedDelete = w.playedDelete,
                            playedChange = w.playedChange,
                            playedAdd = w.playedAdd,
                        });
                    }

                    archive.paragraphs.Add(snap);
                }
            }
            string json = JsonUtility.ToJson(archive, true);  
            if (type == 0)//autoSave
            {
                File.WriteAllText(Path.Combine(Application.persistentDataPath, AutoSaveFile), json);    
            }
            else
            {
                File.WriteAllText(Path.Combine(Application.persistentDataPath, PersonSaveFile), json);
            }   
        }
        private static readonly string AutoSaveFile = "AutoSaveSentenceManagerData.json";
        private static readonly string PersonSaveFile = "PersonSaveSentenceManagerData.json";
        #endregion

        public int line = 0;
        #region 恢复存档
        private void RestoreArchive(Archive data)
        {
            sentenceBeginPlace = data.sentenceBeginPlace;
            paragraphs = new List<Paragraph>();
            guideTime = false;
            line = sentenceBeginPlace;
            CreateSentence(-1);
            
            for (int i = 0; i < paragraphs.Count; i++)
            {
                var s   = paragraphs[i];
                var ss  = data.paragraphs[i];

                paragraphNow = data.paragraphNow;
                foreach (var page in s.pages)
                {
                    for (int j = 0; j < page.words.Count; j++)
                    {
                        var w  = page.words[j];
                        var ws = ss.words[j];
                        w.wordType = ws.type;
                        w.enable = ws.enable;
                        w.wordText.text   = ws.currentText;
                        w.playedDelete    = ws.playedDelete;
                        w.playedChange    = ws.playedChange;
                        w.playedAdd       = ws.playedAdd;
                        w.hasSpecial2NextPara = ws.hasSpecial2NextPara;
                        w.hasRun          = ws.hasRun;
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

        public void DisAbleThisPara()
        {
            paragraphs[paragraphNow].gameObject.SetActive(false);
        }
        public void NextPara(int nextParagraphNumber)
        {
            paragraphs[paragraphNow].gameObject.SetActive(false);
            paragraphNow = nextParagraphNumber;
            paragraphs[paragraphNow].gameObject.SetActive(true);
        }
        private void CreateSentence(int fatherSentenceNumber)
        {
            GameObject cloneParagraph = Instantiate(PygmalionGameManager.Instance.paragraphCloneObj,transform);
            cloneParagraph.GetComponent<Paragraph>().fatherSentenceNumber = fatherSentenceNumber;
            cloneParagraph.GetComponent<Paragraph>().pages = new List<Page>();
            cloneParagraph.GetComponent<Paragraph>().sentenceNumber = paragraphs.Count;
            paragraphs.Add(cloneParagraph.GetComponent<Paragraph>());
            string l = ResourceLoader.textLoader[DataManager.Instance.ScriptNow].Lines[line++];
            print(l);
            Dictionary<string, string> parsedTag = Utils.ParseLine(l);
            while (!string.Equals(parsedTag["tag"], "end", StringComparison.Ordinal))
            {
                if(parsedTag["tag"]=="sentenceEnd")
                {
                    switch (parsedTag["type"])
                    {
                        case "0":
                            type = ConfirmType.Normal;
                            for (int i = 0; i < 3; i++)
                            {
                                endScriptsList.Add(ResourceLoader.textLoader[DataManager.Instance.ScriptNow].Lines[line++].TrimStart().Trim());
                            }
                            break;
                        case "1":
                            type = ConfirmType.OnlyOneCorrect;
                            for (int i = 0; i < 2; i++)
                            {
                                endScriptsList.Add(ResourceLoader.textLoader[DataManager.Instance.ScriptNow].Lines[line++].TrimStart().Trim());
                            }
                            break; 
                           
                    } 
                    l = ResourceLoader.textLoader[DataManager.Instance.ScriptNow].Lines[line++];
                }
                else if (parsedTag["tag"] == "page")
                {
                    GameObject page = Instantiate(cloneParagraph.GetComponent<Paragraph>().PageCloneGameObject,cloneParagraph.GetComponent<Paragraph>().pagesFather.transform);
                    cloneParagraph.GetComponent<Paragraph>().pages.Add(page.GetComponent<Page>());
                    l = ResourceLoader.textLoader[DataManager.Instance.ScriptNow].Lines[line++];
                    print(DataManager.Instance.LineNow+" l "+l);
                    parsedTag = Utils.ParseLine(l);
                    Page pageScript =page.GetComponent<Page>();  
                    pageScript.paragraph = cloneParagraph.GetComponent<Paragraph>(); 
                    while (!string.Equals(parsedTag["tag"], "pageEnd", StringComparison.Ordinal))
                    {
                        GameObject word = Instantiate(PygmalionGameManager.Instance.wordCloneObj,pageScript.layout.gameObject.transform);
                        word.SetActive(true);
                        switch (parsedTag["type"])
                        {
                          case "0":
                              word.GetComponent<Word>().wordType = 0;
                              word.GetComponent<Word>().wordText.text = parsedTag["content"];
                              if (parsedTag.ContainsKey("special"))
                                  word.GetComponent<Word>().special = true;
                              word.GetComponent<Word>().wordText.color = Color.white;
                              print( parsedTag["content"]);
                              break;
                          case "1":
                              word.GetComponent<Word>().wordType = 1;
                              word.GetComponent<Word>().spaceYellow.SetActive(true);
                              word.GetComponent<Word>().wordText.text = "<color=#00000000>空</color>";
                              word.GetComponent<Word>().wordText.color = Color.yellow;;
                              word.GetComponent<Word>().addText =  parsedTag["content"];
                              word.GetComponent<Word>().changeWordList = new List<string>();
                              word.GetComponent<Word>().changeWordList.Add("<color=#00000000>空</color>");
                              switch (parsedTag["right"])
                              {
                                  case "0"://add
                                      word.GetComponent<Word>().answerList.Add(parsedTag["content"]);
                                      break;
                                  case "1"://none
                                      word.GetComponent<Word>().answerList.Add("<color=#00000000>空</color>");
                                      break;
                                  case "2"://none and add
                                      word.GetComponent<Word>().answerList.Add(parsedTag["content"]);
                                      word.GetComponent<Word>().answerList.Add("<color=#00000000>空</color>");
                                      break;
                              }
                              word.GetComponent<Word>().doubleClick1Board.GetComponent<DoubleClick1Board>().Gen(word.GetComponent<Word>().changeWordList);
                              break;
                          case "2":
                              word.GetComponent<Word>().wordType = 2;
                              word.GetComponent<Word>().wordText.text = parsedTag["content"];
                              word.GetComponent<Word>().wordText.color = Color.yellow;
                              word.GetComponent<Word>().changeWordList = new List<string>();
                              word.GetComponent<Word>().changeWordList.Add(parsedTag["content"]);
                              for (int i = 0; i < int.Parse(parsedTag["changeNumber"]); i++)
                              {
                                  l = ResourceLoader.textLoader[DataManager.Instance.ScriptNow].Lines[line++];
                                  l = l.TrimStart();
                                  word.GetComponent<Word>().changeWordList.Add(l);
                                  Debug.Log(l);
                              }
                              if (parsedTag.ContainsKey("changeDialog"))
                              {
                                  word.GetComponent<Word>().changeDialog = true;
                                  word.GetComponent<Word>().scriptLine = int.Parse(parsedTag["scriptLine"]); ;
                                  word.GetComponent<Word>().scriptName = parsedTag["scriptName"];
                              }
                              if(parsedTag.ContainsKey("right"))
                                  switch (parsedTag["right"])
                                  {
                                      case "0"://change
                                          for (int i = 1; i < word.GetComponent<Word>().changeWordList.Count; i++)
                                          {
                                              word.GetComponent<Word>().answerList.Add(word.GetComponent<Word>().changeWordList[i]);
                                          }
                                          break;
                                      case "1"://delete
                                          word.GetComponent<Word>().answerList.Add("/");
                                          break;
                                      case "2"://change and delete
                                          word.GetComponent<Word>().answerList.Add("/");
                                          for (int i = 1; i < word.GetComponent<Word>().changeWordList.Count; i++)
                                          {
                                              word.GetComponent<Word>().answerList.Add(word.GetComponent<Word>().changeWordList[i]);
                                          }
                                          break;
                                      case "3"://none
                                          word.GetComponent<Word>().answerList.Add(parsedTag["content"]);
                                          break;
                                  }
                              word.GetComponent<Word>().doubleClick2Board.GetComponent<DoubleClick2Board>().Gen(word.GetComponent<Word>().changeWordList);
                              break;
                          case "3":
                              word.GetComponent<Word>().wordType = 3;
                              word.GetComponent<Word>().wordText.text = parsedTag["content"];
                              word.GetComponent<Word>().wordText.color = Color.red;
                              word.GetComponent<Word>().nextParagraphNumber = SentenceManager.instance.paragraphs.Count;
                              l = ResourceLoader.textLoader[DataManager.Instance.ScriptNow].Lines[line++];
                              if (parsedTag.ContainsKey("script"))
                              {
                                  word.GetComponent<Word>().hasSpecial2NextPara = true;
                              }
                              if(parsedTag.ContainsKey("special"))
                                    word.GetComponent<Word>().special = true;
                              print("create scentence "+ SentenceManager.instance.paragraphs.Count);
                              CreateSentence(cloneParagraph.GetComponent<Paragraph>().sentenceNumber);
                              break;
                          case "4":
                              word.GetComponent<Word>().wordType = 4;
                              word.GetComponent<Word>().wordText.text = parsedTag["content"];
                              word.GetComponent<Word>().wordText.color = Color.yellow;
                              break;
                          case "5":
                              word.GetComponent<Word>().wordType = 5;
                              word.GetComponent<Word>().wordText.text = parsedTag["content"];
                              word.GetComponent<Word>().wordText.color = Color.yellow;
                              break;
                          case "6":
                              word.GetComponent<Word>().wordType = 6;
                              word.GetComponent<Word>().wordText.text = parsedTag["content"];
                              word.GetComponent<Word>().wordText.color = Color.red;
                              if (parsedTag.ContainsKey("end"))
                                  word.GetComponent<Word>().endText = parsedTag["end"];
                              else
                                  word.GetComponent<Word>().endText = "";
                              word.GetComponent<Word>().pic = parsedTag["pic"];
                              l = ResourceLoader.textLoader[DataManager.Instance.ScriptNow].Lines[line++];
                              parsedTag = Utils.ParseLine(l);
                              word.GetComponent<Word>().dialogList = new List<Word.Dialog>();
                              while(parsedTag["tag"] != "dialogEnd")
                              {
                                  word.GetComponent<Word>().dialogList.Add(new Word.Dialog(){Name = parsedTag["role"],Text = parsedTag["content"]});
                                  l = ResourceLoader.textLoader[DataManager.Instance.ScriptNow].Lines[line++];
                                  parsedTag = Utils.ParseLine(l);
                              }
                              break;
                    }
                        word.GetComponent<AutoBox>().RefreshBox2d();
                        word.SetActive(true);                
                        pageScript.words.Add(word.GetComponent<Word>());
                        word.GetComponent<Word>().page = pageScript;
                        l = ResourceLoader.textLoader[DataManager.Instance.ScriptNow].Lines[line++];
                        parsedTag = Utils.ParseLine(l);
                    }
                    pageScript.layout.Refresh();
                    pageScript.gameObject.SetActive(pageScript.paragraph.pages.Count==1);
                }
                l = ResourceLoader.textLoader[DataManager.Instance.ScriptNow].Lines[line++];
                parsedTag = Utils.ParseLine(l);    
                print(l);
            }
            cloneParagraph.GetComponent<Paragraph>().Refresh();    
            cloneParagraph.SetActive(fatherSentenceNumber == -1);    
            cloneParagraph.GetComponent<Paragraph>().Appear();
        }

        public void StartNewGame()
        {
            if (File.Exists(Path.Combine(Application.persistentDataPath, AutoSaveFile)))
                File.Delete(Path.Combine(Application.persistentDataPath, AutoSaveFile));
            if (File.Exists(Path.Combine(Application.persistentDataPath, PersonSaveFile)))
                File.Delete(Path.Combine(Application.persistentDataPath, PersonSaveFile));
            for (int i = 0; i < paragraphs.Count; i++)
            {
                var s   = paragraphs[i];
                foreach (var page in s.pages)
                {
                    for (int j = 0; j < page.words.Count; j++)
                    {
                       Destroy(page.words[j].gameObject); 
                    }
                    Destroy(page.gameObject);
                }
                Destroy(paragraphs[i].gameObject);
            }
        }

        public void ContinueGame(int i)
        {
            string json;
            Archive data;
            if (i == 0)
            {
                json = File.ReadAllText(Path.Combine(Application.persistentDataPath, AutoSaveFile)); 
                data = JsonUtility.FromJson<Archive>(json);
            }
            else
            {
                json = File.ReadAllText(Path.Combine(Application.persistentDataPath, PersonSaveFile)); 
                data = JsonUtility.FromJson<Archive>(json);
            }
            RestoreArchive(data);
        }
    }