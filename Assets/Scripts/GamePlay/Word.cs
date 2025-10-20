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
        //点击词语，根据词语的种类不同应该是有不同的表现
        public Sentence sentence;
        public int wordType;
        public TMP_Text wordText; 
        public int sentenceNumber;
        public string addText;
        public List<string> changeWordList;
        public int nextSentenceNumber;
        public bool enable;
        public GameObject click2Board;
        public GameObject doubleClick2Board;
        public bool guideTime = false;
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
        public void OnPointerClick(PointerEventData eventData)
        {
            if (enable)
            {
                float now = Time.unscaledTime;

                if (now - _lastClickTime <= doubleClickInterval)
                {
                    // 双击
                    Debug.Log("双击 Text");
                    OnDoubleClick();
                }
                else
                {
                    // 单击（会先在第一次点击时触发，间隔不到再取消）
                    Invoke(nameof(OnSingleClick), doubleClickInterval);
                }

                _lastClickTime = now;
            }
        }
        private void OnSingleClick()
        {
            // 如果计时过程中又点了一次，则取消本次单击
            float now = Time.unscaledTime;
            if (now - _lastClickTime <= doubleClickInterval) return;
            Debug.Log("单击 Text："+wordText.text);
            switch (wordType)
            {
                case 0:
                    break;
                case 1://add
                    break;
                case 2://替换或者删除
                    click2Board.SetActive(true);
                    break;
                case 3://
                    break;
            }
   
        }
        private void OnDoubleClick()
        {
            // 取消即将执行的单击
            CancelInvoke(nameof(OnSingleClick));
            switch (wordType)
            {
                case 0:
                    break;
                case 1://add
                    break;
                case 2://替换或者删除
                    doubleClick2Board.GetComponent<DoubleClick2Board>().Show(changeWordList);
                    doubleClick2Board.SetActive(true);
                    break;
                case 3://
                    break;
            }
        }

        //小游戏结束后调用，开始处理
        public void ConfirmDeleteWord()
        {
            //变为“/”
            wordText.text = "/";
            RefreshBox2d();
            sentence.gameObject.GetComponent<FlowLayoutGroupCentered>().Refresh();
            if (guideTime)
            {
                GameManager.Instance.ReadLine();
            }
            else
            {
                
            }
        }

        public void ConfirmChangeWord()
        {
            //变成changewordlist的第一个词
            wordText.text = changeWordList[0];
            RefreshBox2d();
            sentence.gameObject.GetComponent<FlowLayoutGroupCentered>().Refresh();
            if (guideTime)
            {
                GameManager.Instance.ReadLine();
            }
            else
            {
                
            }
        }

        public void ConfirmAddWord()
        {
            wordText.text = addText;
            RefreshBox2d();
            sentence.gameObject.GetComponent<FlowLayoutGroupCentered>().Refresh();
            if (guideTime)
            {
                GameManager.Instance.ReadLine();
            }
            else
            {
                
            }
        }

        public void Fade(float time)
        {
            wordText.DOFade(0, time);
        }
    }
}
