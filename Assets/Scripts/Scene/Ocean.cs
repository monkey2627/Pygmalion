using UnityEngine;
using UnityEngine.Serialization;

namespace Scene
{
    public class Ocean : Scene
    {
        public GameObject backGround;
        public GameObject vp;
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
