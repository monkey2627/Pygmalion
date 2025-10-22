using System;
using DG.Tweening;
using UnityEngine;

namespace Scene
{
    public class Guide : MonoBehaviour
    {
        public TMPro.TMP_Text text;
        public bool enable = true;

        private void OnEnable()
        {
            text.DOColor(new Color(text.color.r,text.color.g, text.color.b, 0.5f), 0.2f)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine);
        }

        public void Click()
        {
            if(!enable)
                return;
            enable = false;
            text.text = "潜力与收益";
            text.DOKill();
            GameObject.Find("delay").transform.DOMove(new Vector3(1000, 1000),2).OnComplete(PygmalionGameManager.Instance.ReadLine);
        }
    
    }
}
