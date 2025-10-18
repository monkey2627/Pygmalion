using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GamePlay
{
    public class TextClick: MonoBehaviour,IPointerClickHandler
    {
        public Word word;
        public void OnPointerClick(PointerEventData eventData)
        {
            word.wordText.text = GetComponent<TMP_Text>().text;
        }
    }
}