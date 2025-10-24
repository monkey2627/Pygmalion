using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace GamePlay
{
    public class Word : MonoBehaviour,IPointerClickHandler
    {
        public struct Dialog
        {
            public string Name;
            public string Text;
        }
        //点击词语，根据词语的种类不同应该是有不同的表现
        public Page page;
        public int wordType;
        public TMP_Text wordText; 
        public string addText;
        public List<string> changeWordList;
        //Type = 2时，同时显示的还有黄色的色块
        public GameObject spaceYellow;
        //Type = 3时，双击词语会跳转到的句子编号
        public int nextParagraphNumber;
        //Type = 6时，所对应的静态图名字
        public string pic;
        public List<Dialog> dialogList;
        public string endText;
        public bool enable = false;
        public GameObject click2Board;
        public GameObject deleteChoice;
        public GameObject changeChoice;
        public GameObject click1Board;
        public GameObject click0Board;
        public GameObject doubleClick2Board;
        public GameObject doubleClick1Board;
        public bool guideTime = false;
        //一个单词只能玩儿一次对应操作小游戏，一旦玩儿过一次就不能再玩儿这个操作的游戏了
        public bool playedDelete = false;
        public bool playedChange = false;
        public bool playedAdd = false;
        //存所有可以算作是正确的选项
        public List<String> answerList;
        //在做完相应操作之后有没有对话
        public bool changeDialog = false;
        public bool deleteDialog = false;
        public bool addDialog = false;
        public string scriptName;
        public int scriptLine;

        public void RefreshBox2d()
        {
            var tmp = GetComponent<TMP_Text>();
            var box2d = GetComponent<BoxCollider2D>();
            if (tmp == null || box2d == null) return;
            // 确保 TMP 已计算完宽高
            tmp.ForceMeshUpdate();

            float w = tmp.preferredWidth;
            float h = tmp.preferredHeight;

            box2d.size   = new Vector2(w, h);
            box2d.offset = Vector2.zero;   // 以文本 Pivot 为中心
        }
        
        [Tooltip("两次点击间隔小于多少秒算双击")]
        public float doubleClickInterval = 0.2f;
        private float _lastClickTime = -1f;
        public bool isSingleClick = false;
        public bool isPanel = false;
        public void Close()
        {
            isPanel = false;
            click2Board.SetActive(false);
            click1Board.SetActive(false);
            click0Board.SetActive(false);
            doubleClick2Board.SetActive(false);
            doubleClick1Board.SetActive(false);
        }

        private void Awake()
        {
            scriptName = "none";
        }

        private void OnEnable()
        {
            GetComponent<AutoBox>().RefreshBox2d();
        }
        public bool IsRight()
        {
            foreach (var ans in answerList)
            {
                if (ans == wordText.text)
                {
                    return true;
                }
            }
            return false;
        }
        public void OnPointerClick(PointerEventData eventData)
        {
            if (enable && !isPanel)
            {
                var now = Time.time;
                if (now - _lastClickTime <= doubleClickInterval)
                {
                    Debug.Log("双击 Text");
                    OnDoubleClick();
                    isSingleClick = false;
                }
                else
                {
                    isSingleClick = true;
                    Invoke(nameof(OnSingleClick), doubleClickInterval);
                }
                _lastClickTime = now;
            }
        }
        private void OnSingleClick()
        {
            if(!isSingleClick) return;
            PygmalionGameManager.Instance.upperButtons.SetActive(false);
            print("单机"+wordText.text);
            isPanel = true;
            page.ClosePanel();
            switch (wordType)
            {
                case 0://出现”你确定这不是bug？“
                    click0Board.SetActive(true);
                    break;
                case 1://add,增添过后点击就没反应了，不能再增了
                    if(!playedAdd)
                        click1Board.SetActive(true);
                    else
                        PygmalionGameManager.Instance.upperButtons.SetActive(true);
                    break;
                case 2://替换或者删除,同样每种小游戏只能玩儿一次
                    changeChoice.SetActive(!playedChange);
                    deleteChoice.SetActive(!playedDelete);
                    click2Board.SetActive(true);
                    break;
                case 3://单击没反应，双击才有用
                    PygmalionGameManager.Instance.upperButtons.SetActive(true);
                    break;
                case 4://删除
                    changeChoice.SetActive(false);
                    deleteChoice.SetActive(!playedDelete);
                    click2Board.SetActive(true);
                    break;
                case 5://替换
                    changeChoice.SetActive(!playedChange);
                    deleteChoice.SetActive(false);
                    click2Board.SetActive(true);
                    break;
            }
   
        }
        public bool hasSpecial = false;
        public bool hasRun = false;
        public bool click = false;
        private void OnDoubleClick()
        {
            // 取消即将执行的单击
            CancelInvoke(nameof(OnSingleClick));
            if(guideTime) return;
            PygmalionGameManager.Instance.upperButtons.SetActive(false);
            isPanel = true;
            page.ClosePanel();
            switch (wordType)
            {
                case 0://没反应
                    break;
                case 1://add 
                    doubleClick1Board.GetComponent<DoubleClick1Board>().Show(playedAdd,this);
                    doubleClick1Board.SetActive(true);
                    break;
                case 2://替换或者删除
                    doubleClick2Board.GetComponent<DoubleClick2Board>().Show(playedChange,playedDelete,this);
                    doubleClick2Board.SetActive(true);
                    break;
                case 3://进入对应的下一个para
                    if (hasSpecial && !hasRun)
                    {
                        PygmalionGameManager.Instance.Change2ScriptAndReadLine("eSupport",0);
                        hasRun = true;
                    }
                    SentenceManager.instance.NextPara(nextParagraphNumber);
                    break;
                case 4://删除
                    doubleClick2Board.GetComponent<DoubleClick2Board>().Show(false,playedDelete,this);
                    doubleClick2Board.SetActive(true);
                    break;
                case 5://替换
                    doubleClick2Board.GetComponent<DoubleClick2Board>().Show(playedChange,false,this);
                    doubleClick2Board.SetActive(true);
                    break;
                case 6://出现图
                    SentenceDialog.Instance.Show(pic, dialogList, endText);
                    break;
            }
        }

        public GameObject mainCamera;
        public void DeleteGame()
        {
                    Close();
                    
                    //测试用
                    ConfirmDeleteWord();
        }
        public void AddGame()
        {
            Close();
            SentenceManager.instance.wordClicked = this;
            page.paragraph.gameObject.SetActive(false);
            mainCamera.SetActive(false);
            PygmalionGameManager.Instance.dialog.SetActive(false);
            PushBoxGameManager.instance.StartPushBoxGame();
            //测试用
            //ConfirmAddWord();
        }
        
        public void ChangeGame()
        {
                    Close();
                    
                    //测试用
                    ConfirmChangeWord();
        }

        public bool special = false;
        //小游戏结束后调用，开始处理
        public void ConfirmDeleteWord()
        {
            //变为“/”
            wordText.text = "/";
            playedDelete = true;
            RefreshBox2d();
            page.layout.GetComponent<FlowLayoutGroupCentered>().Refresh();
            if (guideTime)
            {
                PygmalionGameManager.Instance.ReadLine();
            }
            else
            {
                PygmalionGameManager.Instance.upperButtons.SetActive(true);
                if (special)
                {
                    foreach (var paragraph in SentenceManager.instance.paragraphs)
                    {
                        foreach (var page in paragraph.pages)
                        {
                            foreach (var w in page.words)
                            {
                                string text = w.wordText.text;
                                text = text.Replace("Orpheus", "/");
                                w.wordText.text = text;
                            }
                            
                        }
                    }

                    enable = false;
                }
            }
        }

        public void ConfirmChangeWord()
        {
            //变成changewordlist的第2个词,第一个是他本身
            wordText.text = changeWordList[1];
            RefreshBox2d();
            playedChange = true;
            page.layout.gameObject.GetComponent<FlowLayoutGroupCentered>().Refresh();
            if (guideTime)
            {
                PygmalionGameManager.Instance.ReadLine();
            }
            else
            {
                PygmalionGameManager.Instance.upperButtons.SetActive(true);
                if (changeDialog)
                {
                    PygmalionGameManager.Instance.Change2ScriptAndReadLine(scriptName,scriptLine);
                }
            }
        }

        
        public void ConfirmAddWord()
        {
            wordText.text = addText;
            RefreshBox2d();
            playedAdd = true;
            spaceYellow.SetActive(false);
            mainCamera.SetActive(true);
            page.layout.gameObject.GetComponent<FlowLayoutGroupCentered>().Refresh();
            PushBoxGameManager.instance.gameObject.SetActive(false);
            if (guideTime)
            {
                page.paragraph.gameObject.SetActive(true);
                PygmalionGameManager.Instance.ReadLine();
            }
            else
            {
                PygmalionGameManager.Instance.upperButtons.SetActive(true);
                page.paragraph.gameObject.SetActive(true);
                if (addDialog)
                {
                    PygmalionGameManager.Instance.Change2ScriptAndReadLine(scriptName, scriptLine);
                }   
            }
        }

        public void Fade(float time)
        {
            wordText.DOFade(0, time);
        }
    }
}
