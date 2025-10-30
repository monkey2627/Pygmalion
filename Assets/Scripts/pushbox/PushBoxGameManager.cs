using GamePlay;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace pushbox
{
    public class PushBoxGameManager : MonoBehaviour
    {
        public static PushBoxGameManager instance;
        public GuideWord guideWord;
        public Word word;
        public int last;
        private void Awake()
        {
            instance = this;
        }

        public GameObject[] cloneGameObjects;

        //
        public void PlayHide()
        {
            GameObject page = Instantiate(cloneGameObjects[3],transform);
            last = 3;
            page.SetActive(true);
        }
        public void Play()
        {
            int i = Random.Range(0, 3);    // 0 ≤ i ≤ 99
            last = i;
            GameObject page = Instantiate(cloneGameObjects[i],transform);
            page.SetActive(true);
        }

        public void PlayLast()
        {
            GameObject page = Instantiate(cloneGameObjects[last],transform);
            page.SetActive(true);
        }
    }
}
