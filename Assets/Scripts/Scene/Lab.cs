namespace Scene
{
    public class Lab : Scene
    {
        public override void Load()
        {
            gameObject.SetActive(true);
        }

        public override void Unload()
        {
            gameObject.SetActive(false);
        }
    }
}
