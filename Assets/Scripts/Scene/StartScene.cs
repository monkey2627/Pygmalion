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
            GameManager.Instance.StartNewGame();
        }
        /// <summary>
        /// 继续之前的游戏
        /// </summary>
        public void ContinueGame()
        {
            gameObject.SetActive(false);
            GameManager.Instance.StartNewGame();
        }
    }
}
