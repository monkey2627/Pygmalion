using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Ani
{
    public class waterAni : MonoBehaviour
    {
        public string target="elpis";
        public List<GameObject> jellyFishes;
        public List<String> relatedWords;
        public static waterAni Instance;
        public Dictionary<string,GameObject> roles;
        public GameObject[] roleArray;
        public GameObject BlackBG;
        private void Awake()
        {
            Instance  = this;
            gameObject.SetActive(false);
            roles  = new Dictionary<string, GameObject>();
            roles["elpis"] =  roleArray[0];
        }

        public void ShowTarget()
        {
            switch (target)
            {
                case "elpis":
                    print("elpis");
                    roles["elpis"].SetActive(true);
                    roles["elpis"].GetComponent<SpriteRenderer>().DOFade(0.3f,0);
                    roles["elpis"].GetComponent<SpriteRenderer>().DOFade(1, 0.5f);
                    break;
            }
        }

        public void ShowJellyfish()
        {
            print("jellyfish:"+jellyFishes.Count);
            for (int i = 0; i < jellyFishes.Count; i++)
            {
                jellyFishes[i].SetActive(true);
                jellyFishes[i].GetComponent<BackgroundWander>().Text.gameObject.SetActive(false);
                jellyFishes[i].GetComponent<BackgroundWander>().Text.text = relatedWords[i];
            }
        
        }

        public GameObject DelayGameObject;
        public void CodeStream()
        {   //代码六闪烁1-1.5,水母消失，水母本身的位置出现白长条矩形和文字，依然移动
            for (int i = 0; i < jellyFishes.Count; i++)
            {
                jellyFishes[i].GetComponent<SpriteRenderer>().enabled = false;
                jellyFishes[i].GetComponent<BackgroundWander>().Text.gameObject.SetActive(true);
            }

            DelayGameObject.transform.DOMove(new Vector3(0, 0, 0), 2).OnComplete(() =>
            {
                //结束后背景变为纯黑,字和水母都出现
                gameObject.SetActive(false);
                BlackBG.SetActive(true);
                foreach (var t in jellyFishes)
                {
                    t.GetComponent<SpriteRenderer>().enabled = true;
                    t.GetComponent<BackgroundWander>().Text.gameObject.SetActive(true);
                    t.GetComponent<BackgroundWander>().Text.DOFade(1, 1);
                }
                GameObject.Find("black").GetComponent<SpriteRenderer>().DOFade(0.9F, 1).OnComplete(() =>
                {
                    GameManager.Instance.ReadLine();
                
                });
            
            });
        
        }
    }
}
