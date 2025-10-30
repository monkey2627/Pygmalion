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
            PygmalionGameManager.Instance.ocean.gameObject.SetActive(false);
            PSceneManager.Instance._currentScene = null;
            BGM.instance.Play("textGame");
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
