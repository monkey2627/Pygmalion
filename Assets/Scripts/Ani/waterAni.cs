using System;
using System.Collections.Generic;
using System.IO;
using DG.Tweening;
using UnityEngine;
namespace Ani
{
    public class WaterAni : MonoBehaviour
    {
        public string target="elpis";
        public GameObject circle;
        
        public void ShowJellyFishs()
        {
            PygmalionGameManager.Instance.ocean.gameObject.SetActive(false);
            TransAniManager.Instance.ShowJellyfish();
        }

        public void ShowCodeStream()
        {
            TransAniManager.Instance.CodeStream();
        }
        public void ShowCircle()
        {
            print("show circle");
            circle.SetActive(true);
            gameObject.SetActive(false);
        }
       
    }
}
