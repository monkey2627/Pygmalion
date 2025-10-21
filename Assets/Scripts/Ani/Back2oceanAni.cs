using UnityEngine;
using UnityEngine.Serialization;

namespace Ani
{
    public class Back2OceanAni : MonoBehaviour
    {
        public static Back2OceanAni Instance;
        private void Awake()
        {
            Instance  = this;
            gameObject.SetActive(false);
        }

        public void Finish()
        {
            gameObject.SetActive(false);
            GameManager.Instance.ReadLine();
        }
    }
}
