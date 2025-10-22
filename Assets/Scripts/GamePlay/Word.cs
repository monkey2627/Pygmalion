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
        public Sentence sentence;
        public int wordType;
        public TMP_Text wordText; 
        public string addText;
        public List<string> changeWordList;
        //Type = 2时，同时显示的还有黄色的色块
        public GameObject spaceYellow;
        //Type = 3时，双击词语会跳转到的句子编号
        public int nextSentenceNumber;
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

        private void RefreshBox2d()
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

        private void OnEnable()
        {
            InitPanels();
            GetComponent<AutoBox>().RefreshBox2d();
        }

        public void InitPanels()
        {
            switch (wordType)
            {
                case 0: //出现”你确定这不是bug？“
                    break;
                case 1: //add,增添过后点击就没反应了，不能再增了
                    doubleClick1Board.GetComponent<DoubleClick1Board>().Gen(changeWordList);
                    break;
                case 2: //替换或者删除
                    doubleClick2Board.GetComponent<DoubleClick2Board>().Gen(changeWordList);
                    break;
                case 3: //单击没反应，双击才有用
                    break;
                case 4: //删除
                    doubleClick2Board.GetComponent<DoubleClick2Board>().Gen(changeWordList);
                    break;
                case 5: //替换
                    doubleClick2Board.GetComponent<DoubleClick2Board>().Gen(changeWordList);
                    break;
                default:
                    break;
            }
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
            print("单机"+wordText.text);
            isPanel = true;
            sentence.ClosePanel();
            switch (wordType)
            {
                case 0://出现”你确定这不是bug？“
                    click0Board.SetActive(true);
                    break;
                case 1://add,增添过后点击就没反应了，不能再增了
                    if(!playedAdd)
                        click1Board.SetActive(true);
                    break;
                case 2://替换或者删除,同样每种小游戏只能玩儿一次
                    changeChoice.SetActive(!playedChange);
                    deleteChoice.SetActive(!playedDelete);
                    click2Board.SetActive(true);
                    break;
                case 3://单击没反应，双击才有用
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
        private void OnDoubleClick()
        {
            // 取消即将执行的单击
            CancelInvoke(nameof(OnSingleClick));
            if(guideTime) return;
            isPanel = true;
            sentence.ClosePanel();
            switch (wordType)
            {
                case 0://没反应
                    break;
                case 1://add 
                    doubleClick1Board.GetComponent<DoubleClick1Board>().Show(playedAdd);
                    break;
                case 2://替换或者删除
                    doubleClick2Board.GetComponent<DoubleClick2Board>().Show(playedChange,playedDelete);
                    doubleClick2Board.SetActive(true);
                    break;
                case 3://进入对应的下一个句子
                    sentence.gameObject.SetActive(false);
                    SentenceManager.Instance.sentenceNow = nextSentenceNumber;
                    SentenceManager.Instance.sentences[nextSentenceNumber].gameObject.SetActive(true);
                    break;
                case 4://删除
                    doubleClick2Board.GetComponent<DoubleClick2Board>().Show(false,playedDelete);
                    doubleClick2Board.SetActive(true);
                    break;
                case 5://替换
                    doubleClick2Board.GetComponent<DoubleClick2Board>().Show(playedChange,false);
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
            SentenceManager.Instance.wordClicked = this;
            sentence.gameObject.SetActive(false);
            mainCamera.SetActive(false);
            PygmalionGameManager.instance.dialog.SetActive(false);
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
        //小游戏结束后调用，开始处理
        public void ConfirmDeleteWord()
        {
            //变为“/”
            wordText.text = "/";
            playedDelete = true;
            RefreshBox2d();
            sentence.layout.GetComponent<FlowLayoutGroupCentered>().Refresh();
            if (guideTime)
            {
                PygmalionGameManager.instance.ReadLine();
            }
            else
            {
                
            }
        }

        public void ConfirmChangeWord()
        {
            //变成changewordlist的第2个词,第一个是他本身
            wordText.text = changeWordList[1];
            RefreshBox2d();
            playedChange = true;
            sentence.layout.gameObject.GetComponent<FlowLayoutGroupCentered>().Refresh();
            if (guideTime)
            {
                PygmalionGameManager.instance.ReadLine();
            }
            else
            {
                if (changeDialog)
                {
                    PygmalionGameManager.instance.Change2ScriptAndReadLine(scriptName,scriptLine);
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
            sentence.layout.gameObject.GetComponent<FlowLayoutGroupCentered>().Refresh();
            if (guideTime)
            {
                sentence.gameObject.SetActive(true);
                PygmalionGameManager.instance.ReadLine();
            }
            else
            {
                sentence.gameObject.SetActive(true);
                if (addDialog)
                {
                    PygmalionGameManager.instance.Change2ScriptAndReadLine(scriptName, scriptLine);
                }   
            }
        }

        public void Fade(float time)
        {
            wordText.DOFade(0, time);
        }
    }
}
