using System.IO;
using DG.Tweening;
using UnityEditor;
using UnityEngine;

namespace Scene
{
    public class StartScene : Scene
    {
        public static StartScene Instance;

        private void Awake()
        {
            Instance = this;
        }

        public override void Load()
        {
            base.Load();
            gameObject.SetActive(true);
        }

        /// <summary>
        /// 从头开始新游戏
        /// </summary>
        public void StartNewGame()
        {
            gameObject.SetActive(false);
            print(PygmalionGameManager.Instance.name);
            PygmalionGameManager.Instance.StartNewGame();
        }
        private static readonly string AutoSaveFile = "AutoSaveGameManagerData.json";
        private static readonly string PersonSaveFile = "PersonSaveGameManagerData.json";
        /// <summary>
        /// 继续之前的游戏
        /// </summary>
        public void ContinueGame()
        {
            if (!File.Exists(Path.Combine(Application.persistentDataPath, AutoSaveFile)) &&
                !File.Exists(Path.Combine(Application.persistentDataPath, PersonSaveFile)))
            {
                //两个存档都不存在，不做任何反应
                return;
            }
            gameObject.SetActive(false);
            PygmalionGameManager.Instance.ContinueGame();
        }
        public void PassStart()
        {
            StartGameIcon.transform.DOScale(new Vector3(0.91f, 0.91f, 0.91f), 0.2f);
        }

        public void PassContinue()
        {
            ContinueGameIcon.transform.DOScale(new Vector3(0.91f, 0.91f, 0.91f), 0.2f);
        }

        public void PassEnd()
        {
            EndGameIcon.transform.DOScale(new Vector3(0.91f, 0.91f, 1.01f), 0.2f);
        }

        public void LeaveStart()
        {
            StartGameIcon.transform.DOScale(new Vector3(0.9f, 0.9f, 0.9f), 0.2f);
        }

        public void LeaveContinue()
        {
            ContinueGameIcon.transform.DOScale(new Vector3(0.9f, 0.9f, 0.9f), 0.2f);
        }

        public void LeaveEnd()
        {
            EndGameIcon.transform.DOScale(new Vector3(0.9f, 0.9f, 0.9f), 0.2f);
        }
        public GameObject StartGameIcon;
        public GameObject ContinueGameIcon;
        public GameObject EndGameIcon;

        public void QuitGame()
        {
#if UNITY_EDITOR
                // 编辑器：停止播放模式
                EditorApplication.isPlaying = false;
#else
        // 真机：正常退出
        Application.Quit();
#endif
            }
        }
    }
