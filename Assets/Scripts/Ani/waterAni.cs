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
        public List<GameObject> jellyFishes;
        public List<String> relatedWords;
        public static WaterAni Instance;
       public List<GameObject> rolesList;
        public Dictionary<string,GameObject> roles;
        public GameObject blackBg;
        private static readonly string AutoSaveFile = "SaveGameManagerData.json";
        private static readonly string PersonSaveFile = "SaveGameManagerData.json";
        [Serializable]
        private class SaveData
        {
            public bool isBackBlack;
            public string target;
            public List<string> relatedWords;
        }

        public void FadeJellyFishs()
        {
            for (int i = 0; i < jellyFishes.Count; i++)
            {
                jellyFishes[i].GetComponent<SpriteRenderer>().DOFade(0,2) ;
                jellyFishes[i].GetComponent<BackgroundWander>().Text.gameObject.SetActive(true);
                jellyFishes[i].GetComponent<BackgroundWander>().Text.DOFade(0,2);
            }
        }
        //自动存的时候不会涉及到存waterani
        public void Save(int type = 0)
        {
            SaveData data = new SaveData
            {
                isBackBlack = blackBg.activeInHierarchy,
                target = this.target,
                relatedWords = this.relatedWords
            };

            string json = JsonUtility.ToJson(data,true);
            if (type == 0)//autoSave
            {
                File.WriteAllText(Path.Combine(Application.persistentDataPath, AutoSaveFile), json);    
            }
            else
            {
                File.WriteAllText(Path.Combine(Application.persistentDataPath, PersonSaveFile), json);
            }   
            Debug.Log("waterain数据已保存");
        }
        private void Awake()
        {
            Instance  = this;
            gameObject.SetActive(false);
            roles  = new Dictionary<string, GameObject>();
            foreach (var VARIABLE in rolesList)
            {
                roles[VARIABLE.name] = VARIABLE;
            }
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
                case "ym":
                    print("ym");
                    roles["ym"].SetActive(true);
                    roles["ym"].GetComponent<SpriteRenderer>().DOFade(0.3f,0);
                    roles["ym"].GetComponent<SpriteRenderer>().DOFade(1, 0.5f);
                    break;
                case "e":
                    print("e");
                    roles["e"].SetActive(true);
                    roles["e"].GetComponent<SpriteRenderer>().DOFade(0.3f,0);
                    roles["e"].GetComponent<SpriteRenderer>().DOFade(1, 0.5f);
                    break;
            }
        }

        public void ShowJellyfish()
        {
            for (int i = 0; i < jellyFishes.Count; i++)
            {
                jellyFishes[i].SetActive(true);
                jellyFishes[i].GetComponent<BackgroundWander>().Text.DOFade(1, 0);
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
                blackBg.SetActive(true);
                foreach (var t in jellyFishes)
                {
                    t.GetComponent<SpriteRenderer>().enabled = true;
                    t.GetComponent<SpriteRenderer>().DOFade(1, 0);
                    t.GetComponent<BackgroundWander>().Text.gameObject.SetActive(true);
                    t.GetComponent<BackgroundWander>().Text.DOFade(1, 1);
                }
                GameObject.Find("black").GetComponent<SpriteRenderer>().DOFade(0.9F, 1).OnComplete(() =>
                {
                    print("go black");
                    PygmalionGameManager.Instance.ReadLine();
                
                });
            
            });
        
        }

        public void StartNewGame()
        {
            if (File.Exists(Path.Combine(Application.persistentDataPath, AutoSaveFile)))
                File.Delete(Path.Combine(Application.persistentDataPath, AutoSaveFile));
            if (File.Exists(Path.Combine(Application.persistentDataPath, PersonSaveFile)))
                File.Delete(Path.Combine(Application.persistentDataPath, PersonSaveFile));
            blackBg.SetActive(false);
            roles["elpis"].SetActive(false);
            roles["e"].SetActive(false);
            roles["ym"].SetActive(false);
            jellyFishes[0].SetActive(false);
            jellyFishes[1].SetActive(false);
            jellyFishes[2].SetActive(false);
            jellyFishes[3].SetActive(false);
            jellyFishes[4].SetActive(false);
        }

        public void ContinueGame(int type)
        { 
            string path = Path.Combine(Application.persistentDataPath, type == 0 ? AutoSaveFile : PersonSaveFile);
            if (!File.Exists(path))
            {
                Debug.Log("[VpManager] 没有找到存档文件，跳过读档");
                return;
            }
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            relatedWords = data.relatedWords;
            target = data.target;
            for (int i = 0; i < jellyFishes.Count; i++)
            {
                jellyFishes[i].SetActive(true);
                jellyFishes[i].GetComponent<BackgroundWander>().Text.gameObject.SetActive(true);
                jellyFishes[i].GetComponent<BackgroundWander>().Text.text = relatedWords[i];
            }
            roles["elpis"].SetActive(false);
            roles["e"].SetActive(false);
            roles["ym"].SetActive(false);
            roles[target].SetActive(true);
            roles[target].GetComponent<SpriteRenderer>().DOFade(1, 1f);
        }

        public void FadeTarget()
        {
            roles[target].GetComponent<SpriteRenderer>().DOFade(0, 1f).OnComplete(() =>
            {
                roles[target].SetActive(false);
            });
        }
    }
}
