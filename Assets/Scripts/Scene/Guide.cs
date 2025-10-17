using DG.Tweening;
using UnityEngine;

namespace Scene
{
    public class Guide : MonoBehaviour
    {
        public TMPro.TMP_Text text;
        public bool enable = true;
        public void Click()
        {
            if(!enable)
                return;
            enable = false;
            text.text = "潜力与收益";
            GameObject.Find("delay").transform.DOMove(new Vector3(1000, 1000),2).OnComplete(GameManager.Instance.ReadLine);
        }
    
    }
}
