namespace Scene
{
    public class Lab : Scene
    {
        public override void Load()
        {
            PSceneManager.Instance._currentScene = this;
            gameObject.SetActive(true);
        }

        public override void Unload()
        {
            gameObject.SetActive(false);
        }
    }
}
