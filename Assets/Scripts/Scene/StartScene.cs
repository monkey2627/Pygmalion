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
            UnityEngine.SceneManagement.SceneManager.LoadScene("Game", UnityEngine.SceneManagement.LoadSceneMode.Single);
            GameManager.Instance.StartNewGame();
        }
        /// <summary>
        /// 继续之前的游戏
        /// </summary>
        public void ContinueGame()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Game", UnityEngine.SceneManagement.LoadSceneMode.Single);
            GameManager.Instance.StartNewGame();
        }
    }
}
