using UnityEngine;
using UnityEngine.Serialization;

namespace Scene
{
    public class Ocean : Scene
    {
        public GameObject backGround;
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
