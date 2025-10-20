using UnityEngine;

namespace Ani
{
    public class Back2oceanAni : MonoBehaviour
    {
        public string target="elpis";
        public static Back2oceanAni Instance;
        public GameObject BlackBG;
        private void Awake()
        {
            Instance  = this;
            gameObject.SetActive(false);
        }

    }
}
