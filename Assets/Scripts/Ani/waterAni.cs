using System;
using System.Collections.Generic;
using System.IO;
using DG.Tweening;
using Scene;
using UnityEngine;
namespace Ani
{
    public class WaterAni : MonoBehaviour
    {
        public string target="elpis";
        public GameObject circle;
        
        public void closeocean()
        {
            PygmalionGameManager.instance.ocean.gameObject.SetActive(false);
            PSceneManager.Instance._currentScene = null;
            if(TransAniManager.Instance.target=="elpis" || TransAniManager.Instance.target=="ym")
                BGM.instance.Play("textGame");
            else
                BGM.instance.Play("emptiness");
        }

        public void ShowCodeStream()
        {
            TransAniManager.Instance.CodeStream();
        }
        /**/
        public void ShowCircle()
        {
            print("show circle");
            circle.GetComponent<SpriteRenderer>().DOFade(1, 0);
            circle.SetActive(true);
            gameObject.SetActive(false);
        }
       
    }
}
