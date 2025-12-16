using UnityEngine;
using UnityEngine.Serialization;

namespace Ani
{
    public class Back2OceanAni : MonoBehaviour
    {
        public static Back2OceanAni instance;
        private void Awake()
        {
            instance  = this;
            gameObject.SetActive(false);
        }

        public void Finish()
        {
            gameObject.SetActive(false);
            PygmalionGameManager.instance.ReadLine();
        }
    }
}
